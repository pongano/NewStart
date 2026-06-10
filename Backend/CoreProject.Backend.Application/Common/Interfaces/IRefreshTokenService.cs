namespace CoreProject.Backend.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    RefreshTokenValue CreateRefreshToken();

    string HashToken(string token);
}

public sealed class RefreshTokenValue
{
    public string Token { get; init; } = string.Empty;

    public string TokenHash { get; init; } = string.Empty;
}
