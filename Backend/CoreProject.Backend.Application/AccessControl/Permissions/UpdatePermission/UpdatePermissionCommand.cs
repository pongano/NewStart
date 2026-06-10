namespace CoreProject.Backend.Application.AccessControl.Permissions.UpdatePermission;

public sealed class UpdatePermissionCommand
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}
