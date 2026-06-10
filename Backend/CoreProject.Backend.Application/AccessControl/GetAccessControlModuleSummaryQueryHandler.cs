namespace CoreProject.Backend.Application.AccessControl;

public sealed class GetAccessControlModuleSummaryQueryHandler
{
    public Task<AccessControlModuleSummaryResponse> HandleAsync(
        GetAccessControlModuleSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var response = new AccessControlModuleSummaryResponse
        {
            ModuleName = "AccessControl",
            Description = "AccessControl groups role, permission, and menu concerns for future authorization and admin UI work.",
            PlannedCapabilities =
            [
                "Role management",
                "Permission catalog management",
                "Menu visibility management"
            ],
            PlannedEntities =
            [
                "Role",
                "Permission",
                "Menu"
            ]
        };

        return Task.FromResult(response);
    }
}
