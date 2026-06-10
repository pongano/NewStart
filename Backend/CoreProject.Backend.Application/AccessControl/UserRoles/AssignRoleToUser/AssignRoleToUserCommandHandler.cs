using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.UserRoles.AssignRoleToUser;

public sealed class AssignRoleToUserCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public AssignRoleToUserCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<UserRoleAssignmentResponse> HandleAsync(
        AssignRoleToUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var role = await ValidateAsync(command.UserId, command.RoleId, cancellationToken);

        var userRole = new UserRole
        {
            UserId = command.UserId,
            RoleId = command.RoleId,
            AssignedAtUtc = _dateTimeProvider.UtcNow,
            AssignedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
        };

        await _applicationDbContext.AddUserRoleAsync(userRole, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new UserRoleAssignmentResponse
        {
            UserId = userRole.UserId,
            RoleId = userRole.RoleId,
            RoleCode = role.Code,
            RoleName = role.Name,
            IsActive = role.IsActive,
            AssignedAtUtc = userRole.AssignedAtUtc,
            AssignedBy = userRole.AssignedBy
        };
    }

    private async Task<Role> ValidateAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        var user = await _applicationDbContext.FindUserAccountByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            errors["userId"] = ["User was not found."];
        }

        var role = await _applicationDbContext.FindRoleByIdAsync(roleId, cancellationToken);
        if (role is null)
        {
            errors["roleId"] = ["Role was not found."];
        }

        if (!errors.Any() && await _applicationDbContext.UserRoleExistsAsync(userId, roleId, cancellationToken))
        {
            errors["userRole"] = ["Role is already assigned to the user."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }

        return role!;
    }
}
