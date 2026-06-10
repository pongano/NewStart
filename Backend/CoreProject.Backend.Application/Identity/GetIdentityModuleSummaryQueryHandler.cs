namespace CoreProject.Backend.Application.Identity;

public sealed class GetIdentityModuleSummaryQueryHandler
{
    public Task<IdentityModuleSummaryResponse> HandleAsync(
        GetIdentityModuleSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var response = new IdentityModuleSummaryResponse
        {
            ModuleName = "Identity",
            Description = "Identity is the account-focused module for user lifecycle and authentication-related work.",
            PlannedCapabilities =
            [
                "User account management",
                "Authentication baseline",
                "Account status management"
            ],
            PlannedEntities =
            [
                "UserAccount"
            ]
        };

        return Task.FromResult(response);
    }
}
