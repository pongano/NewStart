using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class AuthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutBearerToken_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutRequiredPermission_ShouldReturnForbidden()
    {
        var client = _factory.CreateAuthenticatedClient("users.manage");

        var response = await client.GetAsync("/api/roles");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task BootstrapAdmin_ThenLogin_ShouldReturnBearerToken()
    {
        var client = _factory.CreateClient();
        var uniqueId = Guid.NewGuid().ToString("N");
        var bootstrapResponse = await client.PostAsJsonAsync("/api/auth/bootstrap-admin", new BootstrapAdminRequest
        {
            UserName = $"admin-{uniqueId}",
            Email = $"admin-{uniqueId}@example.com",
            DisplayName = "Bootstrap Admin",
            Password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.Created, bootstrapResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Identifier = $"admin-{uniqueId}",
            Password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Bearer", payload.TokenType);
        Assert.False(string.IsNullOrWhiteSpace(payload.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(payload.RefreshToken));
        Assert.Contains("users.manage", payload.Permissions);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.AccessToken);
        var protectedResponse = await client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, protectedResponse.StatusCode);

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
        {
            RefreshToken = payload.RefreshToken
        });

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refreshPayload = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(refreshPayload);
        Assert.False(string.IsNullOrWhiteSpace(refreshPayload.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshPayload.RefreshToken));
        Assert.NotEqual(payload.RefreshToken, refreshPayload.RefreshToken);

        var reusedRefreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
        {
            RefreshToken = payload.RefreshToken
        });

        Assert.Equal(HttpStatusCode.BadRequest, reusedRefreshResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshPayload.AccessToken);
        var changePasswordResponse = await client.PostAsJsonAsync("/api/auth/change-password", new ChangePasswordRequest
        {
            CurrentPassword = "Password123!",
            NewPassword = "Password456!"
        });

        Assert.Equal(HttpStatusCode.NoContent, changePasswordResponse.StatusCode);

        var oldPasswordLoginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Identifier = $"admin-{uniqueId}",
            Password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.BadRequest, oldPasswordLoginResponse.StatusCode);

        var newPasswordLoginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Identifier = $"admin-{uniqueId}",
            Password = "Password456!"
        });

        Assert.Equal(HttpStatusCode.OK, newPasswordLoginResponse.StatusCode);

        var secondBootstrapResponse = await client.PostAsJsonAsync("/api/auth/bootstrap-admin", new BootstrapAdminRequest
        {
            UserName = $"admin-again-{uniqueId}",
            Email = $"admin-again-{uniqueId}@example.com",
            DisplayName = "Bootstrap Admin Again",
            Password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.BadRequest, secondBootstrapResponse.StatusCode);
    }

    private sealed class BootstrapAdminRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    private sealed class LoginRequest
    {
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    private sealed class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    private sealed class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    private sealed class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public IReadOnlyCollection<string> Permissions { get; set; } = [];
    }
}
