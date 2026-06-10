using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Permissions.GetPermissionById;

public sealed class GetPermissionByIdQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetPermissionByIdQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<PermissionResponse?> HandleAsync(
        GetPermissionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var permission = await _applicationDbContext.FindPermissionByIdAsync(query.Id, cancellationToken);
        return permission?.ToResponse();
    }
}
