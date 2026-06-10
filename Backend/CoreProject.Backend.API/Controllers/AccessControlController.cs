using CoreProject.Backend.API.Security;
using CoreProject.Backend.Application.AccessControl;
using CoreProject.Backend.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/access-control")]
[RequirePermission(PermissionCodes.AccessGraphRead)]
public sealed class AccessControlController : ControllerBase
{
    private readonly GetAccessControlModuleSummaryQueryHandler _handler;

    public AccessControlController(GetAccessControlModuleSummaryQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("overview")]
    [ProducesResponseType<AccessControlModuleSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AccessControlModuleSummaryResponse>> GetOverview(CancellationToken cancellationToken)
    {
        var response = await _handler.HandleAsync(new GetAccessControlModuleSummaryQuery(), cancellationToken);
        return Ok(response);
    }
}
