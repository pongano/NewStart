namespace CoreProject.Backend.Application.AccessControl.MenuPermissions.AssignPermissionToMenu;

public sealed class AssignPermissionToMenuCommand
{
    public Guid MenuId { get; init; }

    public Guid PermissionId { get; init; }
}
