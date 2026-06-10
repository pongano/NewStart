using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.Common.Interfaces;

public interface IJwtTokenService
{
    JwtTokenResult CreateAccessToken(UserAccount userAccount, IReadOnlyCollection<string> permissionCodes);
}

public sealed class JwtTokenResult
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }
}
