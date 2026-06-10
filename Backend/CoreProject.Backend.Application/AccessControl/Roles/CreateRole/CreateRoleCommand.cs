namespace CoreProject.Backend.Application.AccessControl.Roles.CreateRole;

public sealed class CreateRoleCommand
{
    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool IsActive { get; init; } = true;
}
