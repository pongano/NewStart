namespace CoreProject.Backend.Application.Identity.Users.CreateUser;

public sealed class CreateUserCommand
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
