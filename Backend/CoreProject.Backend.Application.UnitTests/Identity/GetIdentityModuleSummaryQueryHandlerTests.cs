using CoreProject.Backend.Application.Identity;

namespace CoreProject.Backend.Application.UnitTests.Identity;

public sealed class GetIdentityModuleSummaryQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnIdentitySkeletonSummary()
    {
        var handler = new GetIdentityModuleSummaryQueryHandler();

        var response = await handler.HandleAsync(new GetIdentityModuleSummaryQuery());

        Assert.Equal("Identity", response.ModuleName);
        Assert.Contains("UserAccount", response.PlannedEntities);
        Assert.Contains("Authentication baseline", response.PlannedCapabilities);
    }
}
