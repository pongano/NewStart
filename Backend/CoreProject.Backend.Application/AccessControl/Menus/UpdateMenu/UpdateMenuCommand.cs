namespace CoreProject.Backend.Application.AccessControl.Menus.UpdateMenu;

public sealed class UpdateMenuCommand
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Route { get; init; }

    public string? Icon { get; init; }

    public int SortOrder { get; init; }

    public bool IsVisible { get; init; }

    public Guid? ParentId { get; init; }
}
