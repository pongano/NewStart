using CoreProject.Backend.Application.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/identity")]
public sealed class IdentityController : ControllerBase
{
    private readonly GetIdentityModuleSummaryQueryHandler _handler;

    public IdentityController(GetIdentityModuleSummaryQueryHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("overview")]
    [ProducesResponseType<IdentityModuleSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IdentityModuleSummaryResponse>> GetOverview(CancellationToken cancellationToken)
    {
        var response = await _handler.HandleAsync(new GetIdentityModuleSummaryQuery(), cancellationToken);
        return Ok(response);
    }
}
