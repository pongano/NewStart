namespace CoreProject.Backend.Domain.AccessControl.Entities;

public sealed class RolePermission
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTime GrantedAtUtc { get; set; }

    public string? GrantedBy { get; set; }

    public Role Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}
