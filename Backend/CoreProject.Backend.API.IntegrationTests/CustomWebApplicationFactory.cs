using Microsoft.AspNetCore.Mvc.Testing;

namespace CoreProject.Backend.API.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IServiceProvider ServicesProvider => Services;
}
