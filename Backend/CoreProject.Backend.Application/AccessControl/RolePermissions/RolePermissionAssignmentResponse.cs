namespace CoreProject.Backend.Application.AccessControl.RolePermissions;

public sealed class RolePermissionAssignmentResponse
{
    public Guid RoleId { get; init; }

    public Guid PermissionId { get; init; }

    public string PermissionCode { get; init; } = string.Empty;

    public string PermissionName { get; init; } = string.Empty;

    public DateTime GrantedAtUtc { get; init; }

    public string? GrantedBy { get; init; }
}
