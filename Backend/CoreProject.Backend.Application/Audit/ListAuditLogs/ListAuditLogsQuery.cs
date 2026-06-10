namespace CoreProject.Backend.Application.Audit.ListAuditLogs;

public sealed class ListAuditLogsQuery
{
    public int Limit { get; init; } = 100;
}
