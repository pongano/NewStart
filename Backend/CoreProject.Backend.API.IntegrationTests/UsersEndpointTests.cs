using System.Net;
using System.Net.Http.Json;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class UsersEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreated()
    {
        var uniqueId = Guid.NewGuid().ToString("N");
        var request = new CreateUserRequest
        {
            UserName = $"user-{uniqueId}",
            Email = $"user-{uniqueId}@example.com",
            DisplayName = "Created User",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(payload);
        Assert.Equal(request.UserName, payload.UserName);
        Assert.Equal(request.Email, payload.Email);
        Assert.True(payload.IsActive);
    }

    [Fact]
    public async Task ListUsers_ShouldReturnCreatedUsersInAscendingOrder()
    {
        var prefix = $"list-{Guid.NewGuid():N}";
        await CreateUserAsync($"{prefix}-bravo", $"{prefix}-bravo@example.com", "Bravo");
        await CreateUserAsync($"{prefix}-alpha", $"{prefix}-alpha@example.com", "Alpha");

        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<UserResponse>>();

        Assert.NotNull(payload);

        var matchingUsers = payload
            .Where(x => x.UserName.StartsWith(prefix, StringComparison.Ordinal))
            .Select(x => x.UserName)
            .ToList();

        Assert.Equal(new[] { $"{prefix}-alpha", $"{prefix}-bravo" }, matchingUsers);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnCreatedUser()
    {
        var createdUser = await CreateUserAsync(
            $"get-{Guid.NewGuid():N}",
            $"get-{Guid.NewGuid():N}@example.com",
            "Lookup User");

        var response = await _client.GetAsync($"/api/users/{createdUser.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(payload);
        Assert.Equal(createdUser.Id, payload.Id);
        Assert.Equal(createdUser.UserName, payload.UserName);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateUserName_ShouldReturnBadRequest()
    {
        var uniqueId = Guid.NewGuid().ToString("N");
        var firstRequest = new CreateUserRequest
        {
            UserName = $"dup-user-{uniqueId}",
            Email = $"dup-user-{uniqueId}@example.com",
            DisplayName = "Original User",
            Password = "Password123!"
        };

        var duplicateRequest = new CreateUserRequest
        {
            UserName = firstRequest.UserName,
            Email = $"dup-user-second-{uniqueId}@example.com",
            DisplayName = "Duplicate User",
            Password = "Password123!"
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/users", firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsJsonAsync("/api/users", duplicateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithInvalidEmail_ShouldReturnBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            UserName = $"invalid-{Guid.NewGuid():N}",
            Email = "not-an-email",
            DisplayName = "Invalid Email User",
            Password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(400, payload.Status);
        Assert.Equal("One or more validation errors occurred.", payload.Message);
        Assert.False(string.IsNullOrWhiteSpace(payload.TraceId));
        Assert.NotNull(payload.Errors);
        Assert.True(payload.Errors.Any());
        Assert.Contains(
            payload.Errors.SelectMany(x => x.Value),
            x => string.Equals(x, "Email format is invalid.", StringComparison.Ordinal));
    }

    [Fact]
    public async Task GetUserById_WhenMissing_ShouldReturnStandardizedNotFound()
    {
        var response = await _client.GetAsync($"/api/users/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(404, payload.Status);
        Assert.Equal("User was not found.", payload.Message);
        Assert.False(string.IsNullOrWhiteSpace(payload.TraceId));
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnUpdatedUser()
    {
        var createdUser = await CreateUserAsync(
            $"update-{Guid.NewGuid():N}",
            $"update-{Guid.NewGuid():N}@example.com",
            "Original User");

        var response = await _client.PutAsJsonAsync($"/api/users/{createdUser.Id}", new UpdateUserRequest
        {
            UserName = $"{createdUser.UserName}-updated",
            Email = $"updated-{Guid.NewGuid():N}@example.com",
            DisplayName = "Updated User",
            IsActive = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Updated User", payload.DisplayName);
        Assert.False(payload.IsActive);
    }

    [Fact]
    public async Task UpdateUser_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        var firstUser = await CreateUserAsync(
            $"dup-update-{Guid.NewGuid():N}-one",
            $"dup-update-{Guid.NewGuid():N}-one@example.com",
            "First");
        var secondUser = await CreateUserAsync(
            $"dup-update-{Guid.NewGuid():N}-two",
            $"dup-update-{Guid.NewGuid():N}-two@example.com",
            "Second");

        var response = await _client.PutAsJsonAsync($"/api/users/{secondUser.Id}", new UpdateUserRequest
        {
            UserName = $"{secondUser.UserName}-updated",
            Email = firstUser.Email,
            DisplayName = "Second Updated",
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(400, payload.Status);
        Assert.NotNull(payload.Errors);
        Assert.Contains(payload.Errors.SelectMany(x => x.Value), x => string.Equals(x, "Email already exists.", StringComparison.Ordinal));
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNoContent()
    {
        var createdUser = await CreateUserAsync(
            $"delete-{Guid.NewGuid():N}",
            $"delete-{Guid.NewGuid():N}@example.com",
            "Delete User");

        var response = await _client.DeleteAsync($"/api/users/{createdUser.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/users/{createdUser.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WhenMissing_ShouldReturnStandardizedNotFound()
    {
        var response = await _client.DeleteAsync($"/api/users/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal(404, payload.Status);
        Assert.Equal("User was not found.", payload.Message);
        Assert.False(string.IsNullOrWhiteSpace(payload.TraceId));
    }

    [Fact]
    public async Task ResetUserPassword_ShouldAllowLoginWithNewPassword()
    {
        var uniqueId = Guid.NewGuid().ToString("N");
        var createdUser = await CreateUserAsync(
            $"reset-{uniqueId}",
            $"reset-{uniqueId}@example.com",
            "Reset User");

        var response = await _client.PostAsJsonAsync($"/api/users/{createdUser.Id}/reset-password", new ResetPasswordRequest
        {
            NewPassword = "Password456!"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Identifier = createdUser.UserName,
            Password = "Password456!"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.AccessToken));
    }

    private async Task<UserResponse> CreateUserAsync(string userName, string email, string displayName)
    {
        var response = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            UserName = userName,
            Email = email,
            DisplayName = displayName,
            Password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(payload);
        return payload;
    }

    private sealed class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }

    private sealed class UpdateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }

    private sealed class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    private sealed class LoginRequest
    {
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    private sealed class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
    }

    private sealed class UserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    private sealed class ApiErrorResponse
    {
        public string TraceId { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
