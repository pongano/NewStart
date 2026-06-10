namespace CoreProject.Backend.Application.AccessControl.UserRoles;

public sealed class UserRoleAssignmentResponse
{
    public Guid UserId { get; init; }

    public Guid RoleId { get; init; }

    public string RoleCode { get; init; } = string.Empty;

    public string RoleName { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTime AssignedAtUtc { get; init; }

    public string? AssignedBy { get; init; }
}
