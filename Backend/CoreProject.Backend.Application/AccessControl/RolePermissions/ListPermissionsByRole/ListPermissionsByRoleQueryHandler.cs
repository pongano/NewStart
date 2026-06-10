using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.RolePermissions.ListPermissionsByRole;

public sealed class ListPermissionsByRoleQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListPermissionsByRoleQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<RolePermissionAssignmentResponse>> HandleAsync(
        ListPermissionsByRoleQuery query,
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
        var matchingAssignments = rolePermissions.Where(x => x.RoleId == query.RoleId).ToList();
        var permissions = await _applicationDbContext.ListPermissionsAsync(cancellationToken);
        var permissionMap = permissions.ToDictionary(x => x.Id);

        return matchingAssignments
            .Where(x => permissionMap.ContainsKey(x.PermissionId))
            .OrderBy(x => permissionMap[x.PermissionId].Code, StringComparer.Ordinal)
            .Select(x =>
            {
                var permission = permissionMap[x.PermissionId];
                return new RolePermissionAssignmentResponse
                {
                    RoleId = x.RoleId,
                    PermissionId = x.PermissionId,
                    PermissionCode = permission.Code,
                    PermissionName = permission.Name,
                    GrantedAtUtc = x.GrantedAtUtc,
                    GrantedBy = x.GrantedBy
                };
            })
            .ToList();
    }
}
