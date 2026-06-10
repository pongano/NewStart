using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class IdentityEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IdentityEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task GetIdentityOverview_ShouldReturnExpectedPayload()
    {
        var response = await _client.GetAsync("/api/identity/overview");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<IdentityOverviewTestResponse>();

        Assert.NotNull(payload);
        Assert.Equal("Identity", payload.ModuleName);
        Assert.Contains("UserAccount", payload.PlannedEntities);
    }

    private sealed class IdentityOverviewTestResponse
    {
        public string ModuleName { get; set; } = string.Empty;
        public IReadOnlyCollection<string> PlannedEntities { get; set; } = Array.Empty<string>();
    }
}
