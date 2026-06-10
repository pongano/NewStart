using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.UserRoles.ListRolesByUser;

public sealed class ListRolesByUserQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListRolesByUserQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<UserRoleAssignmentResponse>> HandleAsync(
        ListRolesByUserQuery query,
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
        var matchingAssignments = userRoles.Where(x => x.UserId == query.UserId).ToList();
        var roles = await _applicationDbContext.ListRolesAsync(cancellationToken);
        var roleMap = roles.ToDictionary(x => x.Id);

        return matchingAssignments
            .Where(x => roleMap.ContainsKey(x.RoleId))
            .OrderBy(x => roleMap[x.RoleId].Code, StringComparer.Ordinal)
            .Select(x =>
            {
                var role = roleMap[x.RoleId];
                return new UserRoleAssignmentResponse
                {
                    UserId = x.UserId,
                    RoleId = x.RoleId,
                    RoleCode = role.Code,
                    RoleName = role.Name,
                    IsActive = role.IsActive,
                    AssignedAtUtc = x.AssignedAtUtc,
                    AssignedBy = x.AssignedBy
                };
            })
            .ToList();
    }
}
