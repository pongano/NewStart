using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Menus.ListMenus;

public sealed class ListMenusQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListMenusQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<MenuResponse>> HandleAsync(
        ListMenusQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var menus = await _applicationDbContext.ListMenusAsync(cancellationToken);

        return menus
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Code, StringComparer.Ordinal)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
