using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.UserRoles.ReplaceUserRoles;

public sealed class ReplaceUserRolesCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public ReplaceUserRolesCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<UserRoleAssignmentResponse>> HandleAsync(
        ReplaceUserRolesCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var requestedRoleIds = command.RoleIds.Distinct().ToHashSet();
        var user = await _applicationDbContext.FindUserAccountByIdAsync(command.UserId, cancellationToken);
        var roles = await _applicationDbContext.ListRolesAsync(cancellationToken);
        var roleMap = roles.ToDictionary(x => x.Id);

        var errors = new Dictionary<string, string[]>();
        if (user is null)
        {
            errors["userId"] = ["User was not found."];
        }

        var missingRoleIds = requestedRoleIds.Where(x => !roleMap.ContainsKey(x)).ToList();
        if (missingRoleIds.Any())
        {
            errors["roleIds"] = ["One or more roles were not found."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }

        var existingUserRoles = (await _applicationDbContext.ListUserRolesAsync(cancellationToken))
            .Where(x => x.UserId == command.UserId)
            .ToList();

        foreach (var removed in existingUserRoles.Where(x => !requestedRoleIds.Contains(x.RoleId)))
        {
            await _applicationDbContext.RemoveUserRoleAsync(removed, cancellationToken);
        }

        var existingRoleIds = existingUserRoles.Select(x => x.RoleId).ToHashSet();
        foreach (var addedRoleId in requestedRoleIds.Where(x => !existingRoleIds.Contains(x)))
        {
            await _applicationDbContext.AddUserRoleAsync(new UserRole
            {
                UserId = command.UserId,
                RoleId = addedRoleId,
                AssignedAtUtc = _dateTimeProvider.UtcNow,
                AssignedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
            }, cancellationToken);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        var assignments = (await _applicationDbContext.ListUserRolesAsync(cancellationToken))
            .Where(x => x.UserId == command.UserId)
            .Where(x => roleMap.ContainsKey(x.RoleId))
            .OrderBy(x => roleMap[x.RoleId].Code, StringComparer.Ordinal)
            .Select(x =>
            {
                var role = roleMap[x.RoleId];
                return new UserRoleAssignmentResponse
                {
                    UserId = x.UserId,
                    RoleId = x.RoleId,
                    RoleCode = role.Code,
                    RoleName = role.Name,
                    IsActive = role.IsActive,
                    AssignedAtUtc = x.AssignedAtUtc,
                    AssignedBy = x.AssignedBy
                };
            })
            .ToList();

        return assignments;
    }
}
