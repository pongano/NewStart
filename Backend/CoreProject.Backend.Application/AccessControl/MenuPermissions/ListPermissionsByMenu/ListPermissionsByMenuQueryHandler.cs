using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.MenuPermissions.ListPermissionsByMenu;

public sealed class ListPermissionsByMenuQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListPermissionsByMenuQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<MenuPermissionResponse>> HandleAsync(
        ListPermissionsByMenuQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var menu = await _applicationDbContext.FindMenuByIdAsync(query.MenuId, cancellationToken);
        if (menu is null)
        {
            throw new ValidationException(
                "One or more validation errors occurred.",
                new Dictionary<string, string[]> { ["menuId"] = ["Menu was not found."] });
        }

        var menuPermissions = await _applicationDbContext.ListMenuPermissionsAsync(cancellationToken);

        return menuPermissions
            .Where(x => x.MenuId == query.MenuId)
            .OrderBy(x => x.PermissionId)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
