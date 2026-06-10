namespace CoreProject.Backend.Application.AccessControl.Permissions.CreatePermission;

public sealed class CreatePermissionCommand
{
    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}
