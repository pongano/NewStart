using CoreProject.Backend.API.Security;
using CoreProject.Backend.Application.Audit.ListAuditLogs;
using CoreProject.Backend.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/audit-logs")]
[RequirePermission(PermissionCodes.AuditLogsRead)]
public sealed class AuditLogsController : ControllerBase
{
    private readonly ListAuditLogsQueryHandler _listAuditLogsQueryHandler;

    public AuditLogsController(ListAuditLogsQueryHandler listAuditLogsQueryHandler)
    {
        _listAuditLogsQueryHandler = listAuditLogsQueryHandler;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<AuditLogResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AuditLogResponse>>> List(
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var response = await _listAuditLogsQueryHandler.HandleAsync(
            new ListAuditLogsQuery { Limit = limit },
            cancellationToken);

        return Ok(response);
    }
}
