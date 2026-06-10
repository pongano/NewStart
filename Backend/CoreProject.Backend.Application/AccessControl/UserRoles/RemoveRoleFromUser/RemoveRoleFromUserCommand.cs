namespace CoreProject.Backend.Application.AccessControl.UserRoles.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommand
{
    public Guid UserId { get; init; }

    public Guid RoleId { get; init; }
}
