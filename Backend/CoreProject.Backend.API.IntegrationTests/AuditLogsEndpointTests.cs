using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class AuditLogsEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuditLogsEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task ListAuditLogs_ShouldReturnSuccessfulMutationEvents()
    {
        var roleCode = $"audit-role-{Guid.NewGuid():N}";
        var createResponse = await _client.PostAsJsonAsync("/api/roles", new CreateRoleRequest
        {
            Code = roleCode,
            Name = "Audit Role"
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var response = await _client.GetAsync("/api/audit-logs?limit=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<AuditLogResponse>>();
        Assert.NotNull(payload);
        Assert.Contains(payload, x =>
            string.Equals(x.Action, "Roles.Create", StringComparison.Ordinal)
            && string.Equals(x.Method, "POST", StringComparison.Ordinal)
            && x.StatusCode == 201);
    }

    private sealed class CreateRoleRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private sealed class AuditLogResponse
    {
        public string Action { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
