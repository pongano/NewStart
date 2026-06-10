using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class AccessControlEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AccessControlEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAccessControlOverview_ShouldReturnExpectedPayload()
    {
        var response = await _client.GetAsync("/api/access-control/overview");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<AccessControlOverviewTestResponse>();

        Assert.NotNull(payload);
        Assert.Equal("AccessControl", payload.ModuleName);
        Assert.Contains("Permission", payload.PlannedEntities);
    }

    private sealed class AccessControlOverviewTestResponse
    {
        public string ModuleName { get; set; } = string.Empty;
        public IReadOnlyCollection<string> PlannedEntities { get; set; } = Array.Empty<string>();
    }
}
