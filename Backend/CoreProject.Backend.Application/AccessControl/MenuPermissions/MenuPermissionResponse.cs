namespace CoreProject.Backend.Application.AccessControl.MenuPermissions;

public sealed class MenuPermissionResponse
{
    public Guid MenuId { get; init; }

    public Guid PermissionId { get; init; }

    public DateTime LinkedAtUtc { get; init; }

    public string? LinkedBy { get; init; }
}
