namespace CoreProject.Backend.Application.AccessControl.RolePermissions.RemovePermissionFromRole;

public sealed class RemovePermissionFromRoleCommand
{
    public Guid RoleId { get; init; }

    public Guid PermissionId { get; init; }
}
