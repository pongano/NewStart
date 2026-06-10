using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Users.ListUsers;

public sealed class ListUsersQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListUsersQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IReadOnlyCollection<UserAccountResponse>> HandleAsync(
        ListUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var userAccounts = await _applicationDbContext.ListUserAccountsAsync(cancellationToken);

        return userAccounts
            .OrderBy(x => x.UserName, StringComparer.Ordinal)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
