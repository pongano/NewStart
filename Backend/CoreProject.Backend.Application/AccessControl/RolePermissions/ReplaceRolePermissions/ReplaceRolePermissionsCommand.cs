namespace CoreProject.Backend.Application.AccessControl.RolePermissions.ReplaceRolePermissions;

public sealed class ReplaceRolePermissionsCommand
{
    public Guid RoleId { get; init; }

    public IReadOnlyCollection<Guid> PermissionIds { get; init; } = [];
}
