using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Menus.GetMenuById;

public sealed class GetMenuByIdQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetMenuByIdQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<MenuResponse?> HandleAsync(
        GetMenuByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var menu = await _applicationDbContext.FindMenuByIdAsync(query.Id, cancellationToken);
        return menu?.ToResponse();
    }
}
