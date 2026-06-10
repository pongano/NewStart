using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Users.GetUserById;

public sealed class GetUserByIdQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetUserByIdQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<UserAccountResponse?> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var userAccount = await _applicationDbContext.FindUserAccountByIdAsync(query.Id, cancellationToken);
        return userAccount?.ToResponse();
    }
}
