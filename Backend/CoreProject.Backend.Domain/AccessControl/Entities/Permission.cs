using CoreProject.Backend.Domain.Common.Entities;

namespace CoreProject.Backend.Domain.AccessControl.Entities;

public sealed class Permission : AuditableEntity
{
    public const int CodeMaxLength = 150;
    public const int NameMaxLength = 150;
    public const int DescriptionMaxLength = 500;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];

    public ICollection<MenuPermission> MenuPermissions { get; set; } = [];
}
