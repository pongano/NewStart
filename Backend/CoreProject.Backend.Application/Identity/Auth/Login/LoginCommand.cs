namespace CoreProject.Backend.Application.Identity.Auth.Login;

public sealed class LoginCommand
{
    public string Identifier { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string? IpAddress { get; init; }
}
