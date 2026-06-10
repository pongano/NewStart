using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.RolePermissions.ReplaceRolePermissions;

public sealed class ReplaceRolePermissionsCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public ReplaceRolePermissionsCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<RolePermissionAssignmentResponse>> HandleAsync(
        ReplaceRolePermissionsCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var requestedPermissionIds = command.PermissionIds.Distinct().ToHashSet();
        var role = await _applicationDbContext.FindRoleByIdAsync(command.RoleId, cancellationToken);
        var permissions = await _applicationDbContext.ListPermissionsAsync(cancellationToken);
        var permissionMap = permissions.ToDictionary(x => x.Id);

        var errors = new Dictionary<string, string[]>();
        if (role is null)
        {
            errors["roleId"] = ["Role was not found."];
        }

        var missingPermissionIds = requestedPermissionIds.Where(x => !permissionMap.ContainsKey(x)).ToList();
        if (missingPermissionIds.Any())
        {
            errors["permissionIds"] = ["One or more permissions were not found."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }

        var existingRolePermissions = (await _applicationDbContext.ListRolePermissionsAsync(cancellationToken))
            .Where(x => x.RoleId == command.RoleId)
            .ToList();

        foreach (var removed in existingRolePermissions.Where(x => !requestedPermissionIds.Contains(x.PermissionId)))
        {
            await _applicationDbContext.RemoveRolePermissionAsync(removed, cancellationToken);
        }

        var existingPermissionIds = existingRolePermissions.Select(x => x.PermissionId).ToHashSet();
        foreach (var addedPermissionId in requestedPermissionIds.Where(x => !existingPermissionIds.Contains(x)))
        {
            await _applicationDbContext.AddRolePermissionAsync(new RolePermission
            {
                RoleId = command.RoleId,
                PermissionId = addedPermissionId,
                GrantedAtUtc = _dateTimeProvider.UtcNow,
                GrantedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
            }, cancellationToken);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return (await _applicationDbContext.ListRolePermissionsAsync(cancellationToken))
            .Where(x => x.RoleId == command.RoleId)
            .Where(x => permissionMap.ContainsKey(x.PermissionId))
            .OrderBy(x => permissionMap[x.PermissionId].Code, StringComparer.Ordinal)
            .Select(x =>
            {
                var permission = permissionMap[x.PermissionId];
                return new RolePermissionAssignmentResponse
                {
                    RoleId = x.RoleId,
                    PermissionId = x.PermissionId,
                    PermissionCode = permission.Code,
                    PermissionName = permission.Name,
                    GrantedAtUtc = x.GrantedAtUtc,
                    GrantedBy = x.GrantedBy
                };
            })
            .ToList();
    }
}
