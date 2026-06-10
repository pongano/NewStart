using System.Net;
using System.Net.Http.Json;
using CoreProject.Backend.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class SystemInfoEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SystemInfoEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task GetSystemInfo_ShouldReturnExpectedPayload()
    {
        var response = await _client.GetAsync("/api/system/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<SystemInfoTestResponse>();

        Assert.NotNull(payload);
        Assert.Equal("CoreProject.Backend.API", payload.ServiceName);
        Assert.False(string.IsNullOrWhiteSpace(payload.Environment));
        Assert.False(string.IsNullOrWhiteSpace(payload.Version));
    }

    [Fact]
    public void Services_ShouldResolveApplicationDbContextContract()
    {
        using var scope = _factory.ServicesProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetService<IApplicationDbContext>();

        Assert.NotNull(dbContext);
    }

    [Fact]
    public async Task TriggerError_ShouldReturnStandardizedErrorResponse()
    {
        var response = await _client.GetAsync("/api/system/error");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponseTestModel>();

        Assert.NotNull(payload);
        Assert.Equal(500, payload.Status);
        Assert.Equal("An unexpected error occurred.", payload.Message);
        Assert.False(string.IsNullOrWhiteSpace(payload.TraceId));
    }

    private sealed class SystemInfoTestResponse
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime ServerTimeUtc { get; set; }
        public string Version { get; set; } = string.Empty;
    }

    private sealed class ApiErrorResponseTestModel
    {
        public string TraceId { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
