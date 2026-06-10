using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class RolesEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RolesEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateRole_ShouldReturnCreated()
    {
        var request = new CreateRoleRequest
        {
            Code = $"role-{Guid.NewGuid():N}",
            Name = "Administrator",
            Description = "System administrator"
        };

        var response = await _client.PostAsJsonAsync("/api/roles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<RoleResponse>();

        Assert.NotNull(payload);
        Assert.Equal(request.Code, payload.Code);
        Assert.Equal(request.Name, payload.Name);
        Assert.True(payload.IsActive);
    }

    [Fact]
    public async Task ListRoles_ShouldReturnCreatedRolesInAscendingOrder()
    {
        var prefix = $"roles-{Guid.NewGuid():N}";
        await CreateRoleAsync($"{prefix}-bravo", "Bravo");
        await CreateRoleAsync($"{prefix}-alpha", "Alpha");

        var response = await _client.GetAsync("/api/roles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<RoleResponse>>();

        Assert.NotNull(payload);

        var matchingRoles = payload
            .Where(x => x.Code.StartsWith(prefix, StringComparison.Ordinal))
            .Select(x => x.Code)
            .ToList();

        Assert.Equal(new[] { $"{prefix}-alpha", $"{prefix}-bravo" }, matchingRoles);
    }

    [Fact]
    public async Task GetRoleById_ShouldReturnCreatedRole()
    {
        var createdRole = await CreateRoleAsync($"role-{Guid.NewGuid():N}", "Lookup Role");

        var response = await _client.GetAsync($"/api/roles/{createdRole.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<RoleResponse>();

        Assert.NotNull(payload);
        Assert.Equal(createdRole.Id, payload.Id);
        Assert.Equal(createdRole.Code, payload.Code);
    }

    [Fact]
    public async Task CreateRole_WithDuplicateCode_ShouldReturnBadRequest()
    {
        var code = $"dup-role-{Guid.NewGuid():N}";
        var firstResponse = await _client.PostAsJsonAsync("/api/roles", new CreateRoleRequest
        {
            Code = code,
            Name = "Original"
        });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsJsonAsync("/api/roles", new CreateRoleRequest
        {
            Code = code,
            Name = "Duplicate"
        });

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task GetRoleById_WhenMissing_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/roles/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRole_ShouldReturnUpdatedRole()
    {
        var createdRole = await CreateRoleAsync($"role-{Guid.NewGuid():N}", "Original");

        var response = await _client.PutAsJsonAsync($"/api/roles/{createdRole.Id}", new UpdateRoleRequest
        {
            Code = $"{createdRole.Code}-updated",
            Name = "Updated",
            Description = "Updated role",
            IsActive = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<RoleResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Updated", payload.Name);
        Assert.False(payload.IsActive);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnNoContent()
    {
        var createdRole = await CreateRoleAsync($"role-{Guid.NewGuid():N}", "Delete me");

        var response = await _client.DeleteAsync($"/api/roles/{createdRole.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/roles/{createdRole.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<RoleResponse> CreateRoleAsync(string code, string name)
    {
        var response = await _client.PostAsJsonAsync("/api/roles", new CreateRoleRequest
        {
            Code = code,
            Name = name
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<RoleResponse>();
        Assert.NotNull(payload);
        return payload;
    }

    private sealed class CreateRoleRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    private sealed class UpdateRoleRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    private sealed class RoleResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
