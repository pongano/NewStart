using CoreProject.Backend.Domain.Common.Entities;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Domain.Identity.Entities;

public sealed class UserAccount : AuditableEntity
{
    public const int UserNameMaxLength = 100;
    public const int EmailMaxLength = 255;
    public const int DisplayNameMaxLength = 200;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
