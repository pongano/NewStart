using CoreProject.Backend.Application.AccessControl.Menus;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Roles.ListMenusByRole;

public sealed class ListMenusByRoleQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListMenusByRoleQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<MenuResponse>> HandleAsync(
        ListMenusByRoleQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var role = await _applicationDbContext.FindRoleByIdAsync(query.RoleId, cancellationToken);
        if (role is null)
        {
            throw new ValidationException(
                "One or more validation errors occurred.",
                new Dictionary<string, string[]> { ["roleId"] = ["Role was not found."] });
        }

        var rolePermissions = await _applicationDbContext.ListRolePermissionsAsync(cancellationToken);
        var permissionIds = rolePermissions
            .Where(x => x.RoleId == query.RoleId)
            .Select(x => x.PermissionId)
            .ToHashSet();

        var menuPermissions = await _applicationDbContext.ListMenuPermissionsAsync(cancellationToken);
        var menuIds = menuPermissions
            .Where(x => permissionIds.Contains(x.PermissionId))
            .Select(x => x.MenuId)
            .ToHashSet();

        var menus = await _applicationDbContext.ListMenusAsync(cancellationToken);

        return menus
            .Where(x => menuIds.Contains(x.Id))
            .DistinctBy(x => x.Id)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Code, StringComparer.Ordinal)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
