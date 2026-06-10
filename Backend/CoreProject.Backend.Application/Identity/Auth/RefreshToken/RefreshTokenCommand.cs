namespace CoreProject.Backend.Application.Identity.Auth.RefreshToken;

public sealed class RefreshTokenCommand
{
    public string RefreshToken { get; init; } = string.Empty;

    public string? IpAddress { get; init; }
}
