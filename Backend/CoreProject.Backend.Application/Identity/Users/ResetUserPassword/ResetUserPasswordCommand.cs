namespace CoreProject.Backend.Application.Identity.Users.ResetUserPassword;

public sealed class ResetUserPasswordCommand
{
    public Guid UserId { get; init; }

    public string NewPassword { get; init; } = string.Empty;
}
