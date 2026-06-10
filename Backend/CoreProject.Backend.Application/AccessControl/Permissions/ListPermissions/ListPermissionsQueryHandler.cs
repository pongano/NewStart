using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Permissions.ListPermissions;

public sealed class ListPermissionsQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListPermissionsQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<PermissionResponse>> HandleAsync(
        ListPermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var permissions = await _applicationDbContext.ListPermissionsAsync(cancellationToken);

        return permissions
            .OrderBy(x => x.Code, StringComparer.Ordinal)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
