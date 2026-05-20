using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.API.Common.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        IsAuthenticated = principal?.Identity?.IsAuthenticated ?? false;
        UserId = principal?.Identity?.Name;
    }

    public string? UserId { get; }

    public bool IsAuthenticated { get; }
}
