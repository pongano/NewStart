using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class UserAccessGraphEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserAccessGraphEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAccessGraph_ShouldReturnEffectiveRolesPermissionsAndMenus()
    {
        var user = await CreateUserAsync();
        var role = await CreateRoleAsync($"role-{Guid.NewGuid():N}", "Administrator");
        var permission = await CreatePermissionAsync($"perm-{Guid.NewGuid():N}", "Read users");
        var menu = await CreateMenuAsync($"menu-{Guid.NewGuid():N}", "Users", 10);

        var assignRoleResponse = await _client.PostAsync($"/api/users/{user.Id}/roles/{role.Id}", null);
        Assert.Equal(HttpStatusCode.Created, assignRoleResponse.StatusCode);

        var assignPermissionResponse = await _client.PostAsync($"/api/roles/{role.Id}/permissions/{permission.Id}", null);
        Assert.Equal(HttpStatusCode.Created, assignPermissionResponse.StatusCode);

        var linkMenuResponse = await _client.PostAsync($"/api/menus/{menu.Id}/permissions/{permission.Id}", null);
        Assert.Equal(HttpStatusCode.Created, linkMenuResponse.StatusCode);

        var response = await _client.GetAsync($"/api/users/{user.Id}/access-graph");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<UserAccessGraphResponse>();
        Assert.NotNull(payload);
        Assert.Equal(user.Id, payload.UserId);
        Assert.Equal(role.Id, Assert.Single(payload.Roles).Id);
        Assert.Equal(permission.Id, Assert.Single(payload.Permissions).Id);
        Assert.Equal(menu.Id, Assert.Single(payload.Menus).Id);
    }

    private async Task<UserResponse> CreateUserAsync()
    {
        var uniqueId = Guid.NewGuid().ToString("N");
        var response = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            UserName = $"user-{uniqueId}",
            Email = $"user-{uniqueId}@example.com",
            DisplayName = "Access Graph User"
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

    private async Task<MenuResponse> CreateMenuAsync(string code, string name, int sortOrder)
    {
        var response = await _client.PostAsJsonAsync("/api/menus", new CreateMenuRequest
        {
            Code = code,
            Name = name,
            SortOrder = sortOrder
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<MenuResponse>())!;
    }

    private sealed class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
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

    private sealed class CreateMenuRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    private sealed class UserResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class RoleResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class PermissionResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class MenuResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class UserAccessGraphResponse
    {
        public Guid UserId { get; set; }
        public List<RoleNode> Roles { get; set; } = [];
        public List<PermissionNode> Permissions { get; set; } = [];
        public List<MenuNode> Menus { get; set; } = [];
    }

    private sealed class RoleNode
    {
        public Guid Id { get; set; }
    }

    private sealed class PermissionNode
    {
        public Guid Id { get; set; }
    }

    private sealed class MenuNode
    {
        public Guid Id { get; set; }
    }
}
