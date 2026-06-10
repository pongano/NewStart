namespace CoreProject.Backend.Application.Identity.Auth.ChangePassword;

public sealed class ChangePasswordCommand
{
    public string CurrentPassword { get; init; } = string.Empty;

    public string NewPassword { get; init; } = string.Empty;
}
