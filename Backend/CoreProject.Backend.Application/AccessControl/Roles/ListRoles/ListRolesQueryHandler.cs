using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Roles.ListRoles;

public sealed class ListRolesQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListRolesQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<RoleResponse>> HandleAsync(
        ListRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var roles = await _applicationDbContext.ListRolesAsync(cancellationToken);

        return roles
            .OrderBy(x => x.Code, StringComparer.Ordinal)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
