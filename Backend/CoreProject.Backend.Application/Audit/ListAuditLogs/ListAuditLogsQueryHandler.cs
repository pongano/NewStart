using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Audit.ListAuditLogs;

public sealed class ListAuditLogsQueryHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ListAuditLogsQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public Task<IReadOnlyCollection<AuditLogResponse>> HandleAsync(
        ListAuditLogsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var limit = Math.Clamp(query.Limit, 1, 500);
        var response = _applicationDbContext.AuditLogs
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.Id)
            .Take(limit)
            .Select(x => new AuditLogResponse
            {
                Id = x.Id,
                Action = x.Action,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Method = x.Method,
                Path = x.Path,
                StatusCode = x.StatusCode,
                UserId = x.UserId,
                OccurredAtUtc = x.OccurredAtUtc,
                TraceId = x.TraceId,
                Details = x.Details
            })
            .ToList();

        return Task.FromResult<IReadOnlyCollection<AuditLogResponse>>(response);
    }
}
