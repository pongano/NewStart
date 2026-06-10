using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class MenusEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MenusEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task CreateMenu_ShouldReturnCreated()
    {
        var request = new CreateMenuRequest
        {
            Code = $"menu-{Guid.NewGuid():N}",
            Name = "Dashboard",
            Route = "/dashboard",
            SortOrder = 10
        };

        var response = await _client.PostAsJsonAsync("/api/menus", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<MenuResponse>();

        Assert.NotNull(payload);
        Assert.Equal(request.Code, payload.Code);
        Assert.Equal(request.Route, payload.Route);
    }

    [Fact]
    public async Task ListMenus_ShouldReturnCreatedMenusOrderedBySortOrderThenCode()
    {
        var prefix = $"menu-{Guid.NewGuid():N}";
        await CreateMenuAsync($"{prefix}-users", "Users", 20);
        await CreateMenuAsync($"{prefix}-dashboard", "Dashboard", 10);
        await CreateMenuAsync($"{prefix}-audit", "Audit", 20);

        var response = await _client.GetAsync("/api/menus");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<MenuResponse>>();

        Assert.NotNull(payload);

        var matchingMenus = payload
            .Where(x => x.Code.StartsWith(prefix, StringComparison.Ordinal))
            .Select(x => x.Code)
            .ToList();

        Assert.Equal(
            new[] { $"{prefix}-dashboard", $"{prefix}-audit", $"{prefix}-users" },
            matchingMenus);
    }

    [Fact]
    public async Task GetMenuById_ShouldReturnCreatedMenu()
    {
        var createdMenu = await CreateMenuAsync($"menu-{Guid.NewGuid():N}", "Lookup", 15);

        var response = await _client.GetAsync($"/api/menus/{createdMenu.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<MenuResponse>();

        Assert.NotNull(payload);
        Assert.Equal(createdMenu.Id, payload.Id);
        Assert.Equal(createdMenu.Code, payload.Code);
    }

    [Fact]
    public async Task CreateMenu_WithDuplicateCode_ShouldReturnBadRequest()
    {
        var code = $"menu-{Guid.NewGuid():N}";
        var firstResponse = await _client.PostAsJsonAsync("/api/menus", new CreateMenuRequest
        {
            Code = code,
            Name = "Original"
        });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsJsonAsync("/api/menus", new CreateMenuRequest
        {
            Code = code,
            Name = "Duplicate"
        });

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task AssignPermissionToMenu_AndListPermissions_ShouldSucceed()
    {
        var menu = await CreateMenuAsync($"menu-{Guid.NewGuid():N}", "Secured", 30);
        var permission = await CreatePermissionAsync($"perm.{Guid.NewGuid():N}", "Read secured menu");

        var assignResponse = await _client.PostAsync($"/api/menus/{menu.Id}/permissions/{permission.Id}", null);

        Assert.Equal(HttpStatusCode.Created, assignResponse.StatusCode);

        var listResponse = await _client.GetAsync($"/api/menus/{menu.Id}/permissions");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var payload = await listResponse.Content.ReadFromJsonAsync<List<MenuPermissionResponse>>();

        Assert.NotNull(payload);
        var link = Assert.Single(payload);
        Assert.Equal(menu.Id, link.MenuId);
        Assert.Equal(permission.Id, link.PermissionId);
    }

    [Fact]
    public async Task AssignPermissionToMenu_WithDuplicateLink_ShouldReturnBadRequest()
    {
        var menu = await CreateMenuAsync($"menu-{Guid.NewGuid():N}", "Secured", 40);
        var permission = await CreatePermissionAsync($"perm.{Guid.NewGuid():N}", "Read");

        var firstResponse = await _client.PostAsync($"/api/menus/{menu.Id}/permissions/{permission.Id}", null);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsync($"/api/menus/{menu.Id}/permissions/{permission.Id}", null);

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateMenu_ShouldReturnUpdatedMenu()
    {
        var createdMenu = await CreateMenuAsync($"menu-{Guid.NewGuid():N}", "Original", 10);

        var response = await _client.PutAsJsonAsync($"/api/menus/{createdMenu.Id}", new UpdateMenuRequest
        {
            Code = $"{createdMenu.Code}-updated",
            Name = "Updated",
            Route = "/updated",
            SortOrder = 99,
            IsVisible = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<MenuResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Updated", payload.Name);
        Assert.False(payload.IsVisible);
    }

    [Fact]
    public async Task DeleteMenu_ShouldReturnNoContent()
    {
        var createdMenu = await CreateMenuAsync($"menu-{Guid.NewGuid():N}", "Delete", 11);

        var response = await _client.DeleteAsync($"/api/menus/{createdMenu.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/menus/{createdMenu.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteMenu_WithChildren_ShouldReturnBadRequest()
    {
        var parent = await CreateMenuAsync($"menu-{Guid.NewGuid():N}", "Parent", 1);
        await _client.PostAsJsonAsync("/api/menus", new CreateMenuRequest
        {
            Code = $"menu-{Guid.NewGuid():N}",
            Name = "Child",
            SortOrder = 2,
            ParentId = parent.Id
        });

        var response = await _client.DeleteAsync($"/api/menus/{parent.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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

        var payload = await response.Content.ReadFromJsonAsync<MenuResponse>();
        Assert.NotNull(payload);
        return payload;
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

    private sealed class CreateMenuRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Route { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public bool? IsVisible { get; set; }
        public Guid? ParentId { get; set; }
    }

    private sealed class UpdateMenuRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Route { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public bool? IsVisible { get; set; }
        public Guid? ParentId { get; set; }
    }

    private sealed class CreatePermissionRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    private sealed class MenuResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Route { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    private sealed class PermissionResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    private sealed class MenuPermissionResponse
    {
        public Guid MenuId { get; set; }
        public Guid PermissionId { get; set; }
        public DateTime LinkedAtUtc { get; set; }
        public string? LinkedBy { get; set; }
    }
}
