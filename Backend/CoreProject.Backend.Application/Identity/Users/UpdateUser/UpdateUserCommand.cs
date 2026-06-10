namespace CoreProject.Backend.Application.Identity.Users.UpdateUser;

public sealed class UpdateUserCommand
{
    public Guid Id { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
