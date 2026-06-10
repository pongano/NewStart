namespace CoreProject.Backend.Domain.AccessControl.Entities;

public sealed class UserRole
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime AssignedAtUtc { get; set; }

    public string? AssignedBy { get; set; }

    public CoreProject.Backend.Domain.Identity.Entities.UserAccount User { get; set; } = null!;

    public Role Role { get; set; } = null!;
}
