using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.Menus;

public static class MenuMappings
{
    public static MenuResponse ToResponse(this Menu menu)
    {
        return new MenuResponse
        {
            Id = menu.Id,
            Code = menu.Code,
            Name = menu.Name,
            Route = menu.Route,
            Icon = menu.Icon,
            SortOrder = menu.SortOrder,
            IsVisible = menu.IsVisible,
            ParentId = menu.ParentId,
            CreatedAtUtc = menu.CreatedAtUtc
        };
    }
}
