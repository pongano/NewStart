using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace CoreProject.Backend.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = ResolveConnectionString();

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        var environmentConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (!string.IsNullOrWhiteSpace(environmentConnectionString))
        {
            return environmentConnectionString;
        }

        foreach (var basePath in GetCandidateBasePaths())
        {
            var developmentFile = Path.Combine(basePath, "appsettings.Development.json");
            var defaultFile = Path.Combine(basePath, "appsettings.json");

            var connectionString = TryReadConnectionString(developmentFile) ?? TryReadConnectionString(defaultFile);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }
        }

        return "Host=localhost;Port=5432;Database=coreproject_backend_dev;Username=postgres;Password=postgres";
    }

    private static IEnumerable<string> GetCandidateBasePaths()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        yield return Path.GetFullPath(Path.Combine(currentDirectory, "Backend", "CoreProject.Backend.API"));
        yield return Path.GetFullPath(Path.Combine(currentDirectory, "..", "CoreProject.Backend.API"));
        yield return Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "CoreProject.Backend.API"));
    }

    private static string? TryReadConnectionString(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(filePath));

        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings))
        {
            return null;
        }

        if (!connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection))
        {
            return null;
        }

        return defaultConnection.GetString();
    }
}
