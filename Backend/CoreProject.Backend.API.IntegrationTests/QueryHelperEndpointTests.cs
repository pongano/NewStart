using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class QueryHelperEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public QueryHelperEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task UserPermissionsAndRoleMenus_ShouldReturnEffectiveFrontendHelpers()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var user = await CreateUserAsync(suffix);
        var role = await CreateRoleAsync($"helper-role-{suffix}", "Helper Role");
        var permission = await CreatePermissionAsync($"helper.permission.{suffix}", "Helper Permission");
        var menu = await CreateMenuAsync($"helper-menu-{suffix}", "Helper Menu");

        Assert.Equal(HttpStatusCode.Created, (await _client.PostAsync($"/api/users/{user.Id}/roles/{role.Id}", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await _client.PostAsync($"/api/roles/{role.Id}/permissions/{permission.Id}", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await _client.PostAsync($"/api/menus/{menu.Id}/permissions/{permission.Id}", null)).StatusCode);

        var permissionsResponse = await _client.GetAsync($"/api/users/{user.Id}/permissions");
        Assert.Equal(HttpStatusCode.OK, permissionsResponse.StatusCode);

        var permissions = await permissionsResponse.Content.ReadFromJsonAsync<List<PermissionResponse>>();
        Assert.NotNull(permissions);
        Assert.Contains(permissions, x => x.Id == permission.Id);

        var menusResponse = await _client.GetAsync($"/api/roles/{role.Id}/menus");
        Assert.Equal(HttpStatusCode.OK, menusResponse.StatusCode);

        var menus = await menusResponse.Content.ReadFromJsonAsync<List<MenuResponse>>();
        Assert.NotNull(menus);
        Assert.Contains(menus, x => x.Id == menu.Id);
    }

    [Fact]
    public async Task BulkReplaceAssignmentEndpoints_ShouldReplaceSets()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var user = await CreateUserAsync($"bulk-{suffix}");
        var firstRole = await CreateRoleAsync($"bulk-role-a-{suffix}", "Bulk Role A");
        var secondRole = await CreateRoleAsync($"bulk-role-b-{suffix}", "Bulk Role B");
        var permissionA = await CreatePermissionAsync($"bulk.permission.a.{suffix}", "Bulk Permission A");
        var permissionB = await CreatePermissionAsync($"bulk.permission.b.{suffix}", "Bulk Permission B");
        var menu = await CreateMenuAsync($"bulk-menu-{suffix}", "Bulk Menu");

        var replaceRolesResponse = await _client.PutAsJsonAsync($"/api/users/{user.Id}/roles", new ReplaceUserRolesRequest
        {
            RoleIds = [firstRole.Id, secondRole.Id]
        });

        Assert.Equal(HttpStatusCode.OK, replaceRolesResponse.StatusCode);
        var replacedRoles = await replaceRolesResponse.Content.ReadFromJsonAsync<List<UserRoleAssignmentResponse>>();
        Assert.NotNull(replacedRoles);
        Assert.Equal(2, replacedRoles.Count);

        var replaceRolePermissionsResponse = await _client.PutAsJsonAsync($"/api/roles/{firstRole.Id}/permissions", new ReplacePermissionsRequest
        {
            PermissionIds = [permissionA.Id, permissionB.Id]
        });

        Assert.Equal(HttpStatusCode.OK, replaceRolePermissionsResponse.StatusCode);
        var rolePermissions = await replaceRolePermissionsResponse.Content.ReadFromJsonAsync<List<RolePermissionAssignmentResponse>>();
        Assert.NotNull(rolePermissions);
        Assert.Equal(2, rolePermissions.Count);

        var replaceMenuPermissionsResponse = await _client.PutAsJsonAsync($"/api/menus/{menu.Id}/permissions", new ReplacePermissionsRequest
        {
            PermissionIds = [permissionA.Id, permissionB.Id]
        });

        Assert.Equal(HttpStatusCode.OK, replaceMenuPermissionsResponse.StatusCode);
        var menuPermissions = await replaceMenuPermissionsResponse.Content.ReadFromJsonAsync<List<MenuPermissionResponse>>();
        Assert.NotNull(menuPermissions);
        Assert.Equal(2, menuPermissions.Count);

        var shrinkRolesResponse = await _client.PutAsJsonAsync($"/api/users/{user.Id}/roles", new ReplaceUserRolesRequest
        {
            RoleIds = [secondRole.Id]
        });

        Assert.Equal(HttpStatusCode.OK, shrinkRolesResponse.StatusCode);
        var shrunkRoles = await shrinkRolesResponse.Content.ReadFromJsonAsync<List<UserRoleAssignmentResponse>>();
        Assert.NotNull(shrunkRoles);
        Assert.Single(shrunkRoles);
        Assert.Equal(secondRole.Id, shrunkRoles[0].RoleId);
    }

    private async Task<UserResponse> CreateUserAsync(string suffix)
    {
        var response = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            UserName = $"helper-user-{suffix}",
            Email = $"helper-user-{suffix}@example.com",
            DisplayName = "Helper User",
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

    private async Task<MenuResponse> CreateMenuAsync(string code, string name)
    {
        var response = await _client.PostAsJsonAsync("/api/menus", new CreateMenuRequest
        {
            Code = code,
            Name = name,
            SortOrder = 10
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<MenuResponse>())!;
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

    private sealed class ReplaceUserRolesRequest
    {
        public IReadOnlyCollection<Guid> RoleIds { get; set; } = [];
    }

    private sealed class ReplacePermissionsRequest
    {
        public IReadOnlyCollection<Guid> PermissionIds { get; set; } = [];
    }

    private sealed class UserResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class RoleResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class UserRoleAssignmentResponse
    {
        public Guid RoleId { get; set; }
    }

    private sealed class RolePermissionAssignmentResponse
    {
        public Guid PermissionId { get; set; }
    }

    private sealed class MenuPermissionResponse
    {
        public Guid PermissionId { get; set; }
    }

    private sealed class PermissionResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class MenuResponse
    {
        public Guid Id { get; set; }
    }
}
