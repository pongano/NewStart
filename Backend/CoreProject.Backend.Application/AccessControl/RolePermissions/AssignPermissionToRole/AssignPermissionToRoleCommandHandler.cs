using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.RolePermissions.AssignPermissionToRole;

public sealed class AssignPermissionToRoleCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public AssignPermissionToRoleCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<RolePermissionAssignmentResponse> HandleAsync(
        AssignPermissionToRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var permission = await ValidateAsync(command.RoleId, command.PermissionId, cancellationToken);

        var rolePermission = new RolePermission
        {
            RoleId = command.RoleId,
            PermissionId = command.PermissionId,
            GrantedAtUtc = _dateTimeProvider.UtcNow,
            GrantedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
        };

        await _applicationDbContext.AddRolePermissionAsync(rolePermission, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new RolePermissionAssignmentResponse
        {
            RoleId = rolePermission.RoleId,
            PermissionId = rolePermission.PermissionId,
            PermissionCode = permission.Code,
            PermissionName = permission.Name,
            GrantedAtUtc = rolePermission.GrantedAtUtc,
            GrantedBy = rolePermission.GrantedBy
        };
    }

    private async Task<Permission> ValidateAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        var role = await _applicationDbContext.FindRoleByIdAsync(roleId, cancellationToken);
        if (role is null)
        {
            errors["roleId"] = ["Role was not found."];
        }

        var permission = await _applicationDbContext.FindPermissionByIdAsync(permissionId, cancellationToken);
        if (permission is null)
        {
            errors["permissionId"] = ["Permission was not found."];
        }

        if (!errors.Any() && await _applicationDbContext.RolePermissionExistsAsync(roleId, permissionId, cancellationToken))
        {
            errors["rolePermission"] = ["Permission is already assigned to the role."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }

        return permission!;
    }
}
