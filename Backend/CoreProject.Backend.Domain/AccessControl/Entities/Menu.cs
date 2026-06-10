using CoreProject.Backend.Domain.Common.Entities;

namespace CoreProject.Backend.Domain.AccessControl.Entities;

public sealed class Menu : AuditableEntity
{
    public const int CodeMaxLength = 100;
    public const int NameMaxLength = 150;
    public const int RouteMaxLength = 300;
    public const int IconMaxLength = 100;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Route { get; set; }

    public string? Icon { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    public Guid? ParentId { get; set; }

    public Menu? Parent { get; set; }

    public ICollection<Menu> Children { get; set; } = [];

    public ICollection<MenuPermission> MenuPermissions { get; set; } = [];
}
