using CoreProject.Backend.Domain.Common.Entities;

namespace CoreProject.Backend.Domain.Audit.Entities;

public sealed class AuditLog : BaseEntity
{
    public const int ActionMaxLength = 150;
    public const int EntityTypeMaxLength = 150;
    public const int EntityIdMaxLength = 100;
    public const int MethodMaxLength = 10;
    public const int PathMaxLength = 500;
    public const int UserIdMaxLength = 100;
    public const int TraceIdMaxLength = 100;
    public const int DetailsMaxLength = 1000;

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string Method { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public string? UserId { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string? TraceId { get; set; }

    public string? Details { get; set; }
}
