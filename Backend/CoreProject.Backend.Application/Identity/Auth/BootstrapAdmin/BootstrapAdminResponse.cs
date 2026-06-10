namespace CoreProject.Backend.Application.Identity.Auth.BootstrapAdmin;

public sealed class BootstrapAdminResponse
{
    public Guid UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public Guid RoleId { get; init; }

    public string RoleCode { get; init; } = string.Empty;

    public IReadOnlyCollection<string> PermissionCodes { get; init; } = [];
}
