using CoreProject.Backend.Domain.Common.Entities;

namespace CoreProject.Backend.Domain.AccessControl.Entities;

public sealed class Role : AuditableEntity
{
    public const int CodeMaxLength = 100;
    public const int NameMaxLength = 150;
    public const int DescriptionMaxLength = 500;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
