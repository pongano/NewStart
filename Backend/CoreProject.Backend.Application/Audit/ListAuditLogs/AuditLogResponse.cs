namespace CoreProject.Backend.Application.Audit.ListAuditLogs;

public sealed class AuditLogResponse
{
    public Guid Id { get; init; }

    public string Action { get; init; } = string.Empty;

    public string EntityType { get; init; } = string.Empty;

    public string? EntityId { get; init; }

    public string Method { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public string? UserId { get; init; }

    public DateTime OccurredAtUtc { get; init; }

    public string? TraceId { get; init; }

    public string? Details { get; init; }
}
