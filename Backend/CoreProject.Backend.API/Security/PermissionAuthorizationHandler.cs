using System.Security.Claims;
using CoreProject.Backend.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CoreProject.Backend.API.Security;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public PermissionAuthorizationHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Claims.Any(x =>
            string.Equals(x.Type, JwtTokenService.PermissionClaimType, StringComparison.Ordinal)
            && string.Equals(x.Value, requirement.PermissionCode, StringComparison.Ordinal)))
        {
            context.Succeed(requirement);
            return;
        }

        var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return;
        }

        var user = await _applicationDbContext.FindUserAccountByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return;
        }

        var userRoles = await _applicationDbContext.ListUserRolesAsync();
        var roleIds = userRoles
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToHashSet();

        var rolePermissions = await _applicationDbContext.ListRolePermissionsAsync();
        var permissionIds = rolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.PermissionId)
            .ToHashSet();

        var permissions = await _applicationDbContext.ListPermissionsAsync();
        if (permissions.Any(x => permissionIds.Contains(x.Id) && string.Equals(x.Code, requirement.PermissionCode, StringComparison.Ordinal)))
        {
            context.Succeed(requirement);
        }
    }
}
