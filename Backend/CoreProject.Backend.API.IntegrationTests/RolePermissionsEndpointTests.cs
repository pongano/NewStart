using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class RolePermissionsEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RolePermissionsEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task AssignPermissionToRole_AndListPermissions_ShouldSucceed()
    {
        var role = await CreateRoleAsync();
        var prefix = $"perm-{Guid.NewGuid():N}";
        var alphaPermission = await CreatePermissionAsync($"{prefix}.alpha", "Alpha");
        var bravoPermission = await CreatePermissionAsync($"{prefix}.bravo", "Bravo");

        var firstAssignResponse = await _client.PostAsync($"/api/roles/{role.Id}/permissions/{bravoPermission.Id}", null);
        Assert.Equal(HttpStatusCode.Created, firstAssignResponse.StatusCode);

        var secondAssignResponse = await _client.PostAsync($"/api/roles/{role.Id}/permissions/{alphaPermission.Id}", null);
        Assert.Equal(HttpStatusCode.Created, secondAssignResponse.StatusCode);

        var listResponse = await _client.GetAsync($"/api/roles/{role.Id}/permissions");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var payload = await listResponse.Content.ReadFromJsonAsync<List<RolePermissionAssignmentResponse>>();
        Assert.NotNull(payload);
        Assert.Equal(new[] { alphaPermission.Code, bravoPermission.Code }, payload.Select(x => x.PermissionCode).ToArray());
    }

    [Fact]
    public async Task AssignPermissionToRole_WithDuplicateLink_ShouldReturnBadRequest()
    {
        var role = await CreateRoleAsync();
        var permission = await CreatePermissionAsync($"perm-{Guid.NewGuid():N}", "Read");

        var firstResponse = await _client.PostAsync($"/api/roles/{role.Id}/permissions/{permission.Id}", null);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsync($"/api/roles/{role.Id}/permissions/{permission.Id}", null);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

        var payload = await duplicateResponse.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(400, payload.Status);
        Assert.NotNull(payload.Errors);
        Assert.True(payload.Errors.ContainsKey("rolePermission"));
    }

    [Fact]
    public async Task RemovePermissionFromRole_ShouldReturnNoContent()
    {
        var role = await CreateRoleAsync();
        var permission = await CreatePermissionAsync($"perm-{Guid.NewGuid():N}", "Read");

        var assignResponse = await _client.PostAsync($"/api/roles/{role.Id}/permissions/{permission.Id}", null);
        Assert.Equal(HttpStatusCode.Created, assignResponse.StatusCode);

        var removeResponse = await _client.DeleteAsync($"/api/roles/{role.Id}/permissions/{permission.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        var listResponse = await _client.GetAsync($"/api/roles/{role.Id}/permissions");
        var payload = await listResponse.Content.ReadFromJsonAsync<List<RolePermissionAssignmentResponse>>();
        Assert.NotNull(payload);
        Assert.Empty(payload);
    }

    [Fact]
    public async Task RemovePermissionFromRole_WhenMissing_ShouldReturnStandardizedNotFound()
    {
        var response = await _client.DeleteAsync($"/api/roles/{Guid.NewGuid()}/permissions/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(404, payload.Status);
        Assert.Equal("Role-permission assignment was not found.", payload.Message);
        Assert.False(string.IsNullOrWhiteSpace(payload.TraceId));
    }

    private async Task<RoleResponse> CreateRoleAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/roles", new CreateRoleRequest
        {
            Code = $"role-{Guid.NewGuid():N}",
            Name = "Role Permission Test"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<RoleResponse>())!;
    }

    private async Task<PermissionResponse> CreatePermissionAsync(string code, string name)
    {
        var response = await _client.PostAsJsonAsync("/api/permissions", new CreatePermissionRequest
        {
            Code = code,
            Name = name
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<PermissionResponse>())!;
    }

    private sealed class CreateRoleRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private sealed class CreatePermissionRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private sealed class RoleResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class PermissionResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
    }

    private sealed class RolePermissionAssignmentResponse
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
    }

    private sealed class ApiErrorResponse
    {
        public string TraceId { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
