namespace CoreProject.Backend.Application.AccessControl.RolePermissions.AssignPermissionToRole;

public sealed class AssignPermissionToRoleCommand
{
    public Guid RoleId { get; init; }

    public Guid PermissionId { get; init; }
}
