namespace CoreProject.Backend.Application.AccessControl.Menus.CreateMenu;

public sealed class CreateMenuCommand
{
    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Route { get; init; }

    public string? Icon { get; init; }

    public int SortOrder { get; init; }

    public bool IsVisible { get; init; } = true;

    public Guid? ParentId { get; init; }
}
