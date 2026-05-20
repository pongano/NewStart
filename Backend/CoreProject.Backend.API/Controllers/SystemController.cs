using System.Reflection;
using CoreProject.Backend.Application.SystemInfo;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController : ControllerBase
{
    private readonly GetSystemInfoQueryHandler _handler;
    private readonly IHostEnvironment _hostEnvironment;

    public SystemController(GetSystemInfoQueryHandler handler, IHostEnvironment hostEnvironment)
    {
        _handler = handler;
        _hostEnvironment = hostEnvironment;
    }

    [HttpGet("info")]
    [ProducesResponseType<SystemInfoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemInfoResponse>> GetInfo(CancellationToken cancellationToken)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        var response = await _handler.HandleAsync(
            new GetSystemInfoQuery(),
            serviceName: "CoreProject.Backend.API",
            environmentName: _hostEnvironment.EnvironmentName,
            version: version,
            cancellationToken: cancellationToken);

        return Ok(response);
    }

    [HttpGet("error")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult TriggerError()
    {
        throw new InvalidOperationException("Simulated failure for exception middleware verification.");
    }
}
