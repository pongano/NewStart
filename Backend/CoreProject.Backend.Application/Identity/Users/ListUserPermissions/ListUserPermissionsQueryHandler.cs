using CoreProject.Backend.Application.AccessControl.Permissions;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Users.ListUserPermissions;

public sealed class ListUserPermissionsQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListUserPermissionsQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<PermissionResponse>> HandleAsync(
        ListUserPermissionsQuery query,
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
        var roleIds = userRoles
            .Where(x => x.UserId == query.UserId)
            .Select(x => x.RoleId)
            .ToHashSet();

        var rolePermissions = await _applicationDbContext.ListRolePermissionsAsync(cancellationToken);
        var permissionIds = rolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.PermissionId)
            .ToHashSet();

        var permissions = await _applicationDbContext.ListPermissionsAsync(cancellationToken);

        return permissions
            .Where(x => permissionIds.Contains(x.Id))
            .DistinctBy(x => x.Id)
            .OrderBy(x => x.Code, StringComparer.Ordinal)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
