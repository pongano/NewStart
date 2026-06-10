namespace CoreProject.Backend.Domain.Identity.Entities;

public sealed class RefreshToken
{
    public const int TokenHashMaxLength = 128;
    public const int CreatedByIpMaxLength = 100;
    public const int RevokedByIpMaxLength = 100;

    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public string? CreatedByIp { get; set; }

    public string? RevokedByIp { get; set; }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;

    public UserAccount User { get; set; } = null!;
}
