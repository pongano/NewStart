namespace CoreProject.Backend.Application.AccessControl.UserRoles.ReplaceUserRoles;

public sealed class ReplaceUserRolesCommand
{
    public Guid UserId { get; init; }

    public IReadOnlyCollection<Guid> RoleIds { get; init; } = [];
}
