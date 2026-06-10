namespace CoreProject.Backend.Application.AccessControl.UserRoles.AssignRoleToUser;

public sealed class AssignRoleToUserCommand
{
    public Guid UserId { get; init; }

    public Guid RoleId { get; init; }
}
