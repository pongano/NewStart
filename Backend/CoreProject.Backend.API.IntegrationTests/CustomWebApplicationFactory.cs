using System.IdentityModel.Tokens.Jwt;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using CoreProject.Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IServiceProvider ServicesProvider => Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Jwt:Issuer"] = "CoreProject.Backend",
                ["Authentication:Jwt:Audience"] = "CoreProject.Backend.Client",
                ["Authentication:Jwt:SigningKey"] = TestSigningKey,
                ["Authentication:Jwt:AccessTokenMinutes"] = "60",
                ["Logging:LogLevel:Microsoft.EntityFrameworkCore"] = "Warning"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<DbConnection>();

            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            services.AddSingleton<DbConnection>(connection);

            services.AddDbContext<ApplicationDbContext>((provider, options) =>
            {
                options.UseSqlite(provider.GetRequiredService<DbConnection>());
            });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }

    public HttpClient CreateAuthenticatedClient(params string[] permissionCodes)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateAdminTestToken(permissionCodes));
        return client;
    }

    public static string CreateAdminTestToken(params string[] permissionCodes)
    {
        var permissions = permissionCodes.Length == 0
            ? new[]
            {
                "users.manage",
                "roles.manage",
                "permissions.manage",
                "menus.manage",
                "user_roles.manage",
                "role_permissions.manage",
                "menu_permissions.manage",
                "access_graph.read",
                "audit_logs.read"
            }
            : permissionCodes;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa").ToString()),
            new(ClaimTypes.Name, "integration-test-admin"),
            new(ClaimTypes.Email, "integration-test-admin@example.com")
        };

        claims.AddRange(permissions.Select(x => new Claim("permission", x)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "CoreProject.Backend",
            audience: "CoreProject.Backend.Client",
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private const string TestSigningKey = "TEST_ONLY_SIGNING_KEY_FOR_INTEGRATION_TESTS_123456789";
}
