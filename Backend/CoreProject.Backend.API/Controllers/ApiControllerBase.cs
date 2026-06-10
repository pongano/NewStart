using CoreProject.Backend.API.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected NotFoundObjectResult NotFoundError(string message)
    {
        return NotFound(new ApiErrorResponse
        {
            TraceId = HttpContext.TraceIdentifier,
            Status = StatusCodes.Status404NotFound,
            Message = message
        });
    }
}
