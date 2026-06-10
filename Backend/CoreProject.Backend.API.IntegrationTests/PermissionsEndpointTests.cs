using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class PermissionsEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PermissionsEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task CreatePermission_ShouldReturnCreated()
    {
        var request = new CreatePermissionRequest
        {
            Code = $"perm.{Guid.NewGuid():N}",
            Name = "Read users",
            Description = "Allows reading users"
        };

        var response = await _client.PostAsJsonAsync("/api/permissions", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PermissionResponse>();

        Assert.NotNull(payload);
        Assert.Equal(request.Code, payload.Code);
        Assert.Equal(request.Name, payload.Name);
    }

    [Fact]
    public async Task ListPermissions_ShouldReturnCreatedPermissionsInAscendingOrder()
    {
        var prefix = $"perm-{Guid.NewGuid():N}";
        await CreatePermissionAsync($"{prefix}.write", "Write");
        await CreatePermissionAsync($"{prefix}.read", "Read");

        var response = await _client.GetAsync("/api/permissions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<PermissionResponse>>();

        Assert.NotNull(payload);

        var matchingPermissions = payload
            .Where(x => x.Code.StartsWith(prefix, StringComparison.Ordinal))
            .Select(x => x.Code)
            .ToList();

        Assert.Equal(new[] { $"{prefix}.read", $"{prefix}.write" }, matchingPermissions);
    }

    [Fact]
    public async Task GetPermissionById_ShouldReturnCreatedPermission()
    {
        var createdPermission = await CreatePermissionAsync($"perm.{Guid.NewGuid():N}", "Lookup");

        var response = await _client.GetAsync($"/api/permissions/{createdPermission.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PermissionResponse>();

        Assert.NotNull(payload);
        Assert.Equal(createdPermission.Id, payload.Id);
        Assert.Equal(createdPermission.Code, payload.Code);
    }

    [Fact]
    public async Task CreatePermission_WithDuplicateCode_ShouldReturnBadRequest()
    {
        var code = $"perm.{Guid.NewGuid():N}";
        var firstResponse = await _client.PostAsJsonAsync("/api/permissions", new CreatePermissionRequest
        {
            Code = code,
            Name = "Original"
        });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsJsonAsync("/api/permissions", new CreatePermissionRequest
        {
            Code = code,
            Name = "Duplicate"
        });

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task GetPermissionById_WhenMissing_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/permissions/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePermission_ShouldReturnUpdatedPermission()
    {
        var createdPermission = await CreatePermissionAsync($"perm.{Guid.NewGuid():N}", "Original");

        var response = await _client.PutAsJsonAsync($"/api/permissions/{createdPermission.Id}", new UpdatePermissionRequest
        {
            Code = $"{createdPermission.Code}.updated",
            Name = "Updated",
            Description = "Updated permission"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PermissionResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Updated", payload.Name);
    }

    [Fact]
    public async Task DeletePermission_ShouldReturnNoContent()
    {
        var createdPermission = await CreatePermissionAsync($"perm.{Guid.NewGuid():N}", "Delete");

        var response = await _client.DeleteAsync($"/api/permissions/{createdPermission.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/permissions/{createdPermission.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<PermissionResponse> CreatePermissionAsync(string code, string name)
    {
        var response = await _client.PostAsJsonAsync("/api/permissions", new CreatePermissionRequest
        {
            Code = code,
            Name = name
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PermissionResponse>();
        Assert.NotNull(payload);
        return payload;
    }

    private sealed class CreatePermissionRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    private sealed class UpdatePermissionRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    private sealed class PermissionResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
