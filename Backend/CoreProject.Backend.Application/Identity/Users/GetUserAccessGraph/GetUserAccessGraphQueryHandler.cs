using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Users.GetUserAccessGraph;

public sealed class GetUserAccessGraphQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetUserAccessGraphQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<UserAccessGraphResponse> HandleAsync(
        GetUserAccessGraphQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var user = await _applicationDbContext.FindUserAccountByIdAsync(query.UserId, cancellationToken);
        if (user is null)
        {
            throw new ValidationException(
                "One or more validation errors occurred.",
                new Dictionary<string, string[]> { ["userId"] = ["User was not found."] });
        }

        var userRoles = await _applicationDbContext.ListUserRolesAsync(cancellationToken);
        var roles = await _applicationDbContext.ListRolesAsync(cancellationToken);
        var roleMap = roles.ToDictionary(x => x.Id);

        var assignedRoles = userRoles
            .Where(x => x.UserId == query.UserId)
            .Where(x => roleMap.ContainsKey(x.RoleId))
            .Select(x => roleMap[x.RoleId])
            .DistinctBy(x => x.Id)
            .OrderBy(x => x.Code, StringComparer.Ordinal)
            .ToList();

        var roleIds = assignedRoles.Select(x => x.Id).ToHashSet();
        var rolePermissions = await _applicationDbContext.ListRolePermissionsAsync(cancellationToken);
        var permissions = await _applicationDbContext.ListPermissionsAsync(cancellationToken);
        var permissionMap = permissions.ToDictionary(x => x.Id);

        var assignedPermissions = rolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Where(x => permissionMap.ContainsKey(x.PermissionId))
            .Select(x => permissionMap[x.PermissionId])
            .DistinctBy(x => x.Id)
            .OrderBy(x => x.Code, StringComparer.Ordinal)
            .ToList();

        var permissionIds = assignedPermissions.Select(x => x.Id).ToHashSet();
        var menuPermissions = await _applicationDbContext.ListMenuPermissionsAsync(cancellationToken);
        var menus = await _applicationDbContext.ListMenusAsync(cancellationToken);
        var menuMap = menus.ToDictionary(x => x.Id);

        var assignedMenus = menuPermissions
            .Where(x => permissionIds.Contains(x.PermissionId))
            .Where(x => menuMap.ContainsKey(x.MenuId))
            .Select(x => menuMap[x.MenuId])
            .DistinctBy(x => x.Id)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Code, StringComparer.Ordinal)
            .ToList();

        return new UserAccessGraphResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            DisplayName = user.DisplayName,
            IsActive = user.IsActive,
            Roles = assignedRoles
                .Select(x => new UserAccessGraphRoleResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsActive = x.IsActive
                })
                .ToList(),
            Permissions = assignedPermissions
                .Select(x => new UserAccessGraphPermissionResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToList(),
            Menus = assignedMenus
                .Select(x => new UserAccessGraphMenuResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Route = x.Route,
                    Icon = x.Icon,
                    SortOrder = x.SortOrder,
                    IsVisible = x.IsVisible,
                    ParentId = x.ParentId
                })
                .ToList()
        };
    }
}
