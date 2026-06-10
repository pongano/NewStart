namespace CoreProject.Backend.Application.AccessControl.MenuPermissions.ReplaceMenuPermissions;

public sealed class ReplaceMenuPermissionsCommand
{
    public Guid MenuId { get; init; }

    public IReadOnlyCollection<Guid> PermissionIds { get; init; } = [];
}
