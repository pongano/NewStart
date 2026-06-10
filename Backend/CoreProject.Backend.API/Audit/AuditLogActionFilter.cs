using System.Security.Claims;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.Audit.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreProject.Backend.API.Audit;

public sealed class AuditLogActionFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Delete
    };

    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditLogActionFilter(IApplicationDbContext applicationDbContext, IDateTimeProvider dateTimeProvider)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();
        var httpContext = context.HttpContext;

        if (!ShouldAudit(httpContext, executedContext))
        {
            return;
        }

        var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var controllerName = descriptor?.ControllerName ?? "Unknown";
        var actionName = descriptor?.ActionName ?? "Unknown";

        await _applicationDbContext.AddAuditLogAsync(new AuditLog
        {
            Action = $"{controllerName}.{actionName}",
            EntityType = controllerName,
            EntityId = GetEntityId(context),
            Method = httpContext.Request.Method,
            Path = httpContext.Request.Path.Value ?? string.Empty,
            StatusCode = GetStatusCode(executedContext),
            UserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? httpContext.User.Identity?.Name,
            OccurredAtUtc = _dateTimeProvider.UtcNow,
            TraceId = httpContext.TraceIdentifier
        }, httpContext.RequestAborted);

        await _applicationDbContext.SaveChangesAsync(httpContext.RequestAborted);
    }

    private static bool ShouldAudit(HttpContext httpContext, ActionExecutedContext executedContext)
    {
        var statusCode = GetStatusCode(executedContext);
        return executedContext.Exception is null
            && AuditedMethods.Contains(httpContext.Request.Method)
            && statusCode is >= 200 and < 300;
    }

    private static int GetStatusCode(ActionExecutedContext executedContext)
    {
        return executedContext.Result switch
        {
            ObjectResult objectResult => objectResult.StatusCode ?? StatusCodes.Status200OK,
            StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
            _ => executedContext.HttpContext.Response.StatusCode
        };
    }

    private static string? GetEntityId(ActionExecutingContext context)
    {
        foreach (var key in new[] { "id", "userId", "roleId", "permissionId", "menuId" })
        {
            if (context.RouteData.Values.TryGetValue(key, out var value))
            {
                return value?.ToString();
            }
        }

        return null;
    }
}
