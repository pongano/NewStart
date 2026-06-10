namespace CoreProject.Backend.Application.AccessControl.Menus;

public sealed class MenuResponse
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Route { get; init; }

    public string? Icon { get; init; }

    public int SortOrder { get; init; }

    public bool IsVisible { get; init; }

    public Guid? ParentId { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
