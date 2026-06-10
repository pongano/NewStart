using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.RolePermissions.RemovePermissionFromRole;

public sealed class RemovePermissionFromRoleCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public RemovePermissionFromRoleCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<bool> HandleAsync(RemovePermissionFromRoleCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var rolePermission = await _applicationDbContext.FindRolePermissionAsync(command.RoleId, command.PermissionId, cancellationToken);
        if (rolePermission is null)
        {
            return false;
        }

        await _applicationDbContext.RemoveRolePermissionAsync(rolePermission, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
