using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class UserRolesEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserRolesEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task AssignRoleToUser_AndListRoles_ShouldSucceed()
    {
        var user = await CreateUserAsync();
        var prefix = $"role-{Guid.NewGuid():N}";
        var alphaRole = await CreateRoleAsync($"{prefix}-alpha", "Alpha");
        var bravoRole = await CreateRoleAsync($"{prefix}-bravo", "Bravo");

        var firstAssignResponse = await _client.PostAsync($"/api/users/{user.Id}/roles/{bravoRole.Id}", null);
        Assert.Equal(HttpStatusCode.Created, firstAssignResponse.StatusCode);

        var secondAssignResponse = await _client.PostAsync($"/api/users/{user.Id}/roles/{alphaRole.Id}", null);
        Assert.Equal(HttpStatusCode.Created, secondAssignResponse.StatusCode);

        var listResponse = await _client.GetAsync($"/api/users/{user.Id}/roles");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var payload = await listResponse.Content.ReadFromJsonAsync<List<UserRoleAssignmentResponse>>();
        Assert.NotNull(payload);
        Assert.Equal(new[] { alphaRole.Code, bravoRole.Code }, payload.Select(x => x.RoleCode).ToArray());
    }

    [Fact]
    public async Task AssignRoleToUser_WithDuplicateLink_ShouldReturnBadRequest()
    {
        var user = await CreateUserAsync();
        var role = await CreateRoleAsync($"role-{Guid.NewGuid():N}", "Administrator");

        var firstResponse = await _client.PostAsync($"/api/users/{user.Id}/roles/{role.Id}", null);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsync($"/api/users/{user.Id}/roles/{role.Id}", null);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

        var payload = await duplicateResponse.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(400, payload.Status);
        Assert.NotNull(payload.Errors);
        Assert.True(payload.Errors.ContainsKey("userRole"));
    }

    [Fact]
    public async Task RemoveRoleFromUser_ShouldReturnNoContent()
    {
        var user = await CreateUserAsync();
        var role = await CreateRoleAsync($"role-{Guid.NewGuid():N}", "Administrator");

        var assignResponse = await _client.PostAsync($"/api/users/{user.Id}/roles/{role.Id}", null);
        Assert.Equal(HttpStatusCode.Created, assignResponse.StatusCode);

        var removeResponse = await _client.DeleteAsync($"/api/users/{user.Id}/roles/{role.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        var listResponse = await _client.GetAsync($"/api/users/{user.Id}/roles");
        var payload = await listResponse.Content.ReadFromJsonAsync<List<UserRoleAssignmentResponse>>();
        Assert.NotNull(payload);
        Assert.Empty(payload);
    }

    [Fact]
    public async Task RemoveRoleFromUser_WhenMissing_ShouldReturnStandardizedNotFound()
    {
        var response = await _client.DeleteAsync($"/api/users/{Guid.NewGuid()}/roles/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(404, payload.Status);
        Assert.Equal("User-role assignment was not found.", payload.Message);
        Assert.False(string.IsNullOrWhiteSpace(payload.TraceId));
    }

    private async Task<UserResponse> CreateUserAsync()
    {
        var uniqueId = Guid.NewGuid().ToString("N");
        var response = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            UserName = $"user-{uniqueId}",
            Email = $"user-{uniqueId}@example.com",
            DisplayName = "Role Test User",
            Password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
    }

    private async Task<RoleResponse> CreateRoleAsync(string code, string name)
    {
        var response = await _client.PostAsJsonAsync("/api/roles", new CreateRoleRequest
        {
            Code = code,
            Name = name
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<RoleResponse>())!;
    }

    private sealed class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    private sealed class CreateRoleRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private sealed class UserResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class RoleResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
    }

    private sealed class UserRoleAssignmentResponse
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    private sealed class ApiErrorResponse
    {
        public string TraceId { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
