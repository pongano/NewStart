namespace CoreProject.Backend.Domain.AccessControl.Entities;

public sealed class MenuPermission
{
    public Guid MenuId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTime LinkedAtUtc { get; set; }

    public string? LinkedBy { get; set; }

    public Menu Menu { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}
