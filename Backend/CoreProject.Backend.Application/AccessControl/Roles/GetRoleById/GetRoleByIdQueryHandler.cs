using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Roles.GetRoleById;

public sealed class GetRoleByIdQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetRoleByIdQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<RoleResponse?> HandleAsync(
        GetRoleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var role = await _applicationDbContext.FindRoleByIdAsync(query.Id, cancellationToken);
        return role?.ToResponse();
    }
}
