namespace CoreProject.Backend.Application.Identity.Auth.BootstrapAdmin;

public sealed class BootstrapAdminCommand
{
    public string UserName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
