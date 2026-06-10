using CoreProject.Backend.Application.AccessControl;

namespace CoreProject.Backend.Application.UnitTests.AccessControl;

public sealed class GetAccessControlModuleSummaryQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnAccessControlSkeletonSummary()
    {
        var handler = new GetAccessControlModuleSummaryQueryHandler();

        var response = await handler.HandleAsync(new GetAccessControlModuleSummaryQuery());

        Assert.Equal("AccessControl", response.ModuleName);
        Assert.Contains("Role", response.PlannedEntities);
        Assert.Contains("Menu visibility management", response.PlannedCapabilities);
    }
}
