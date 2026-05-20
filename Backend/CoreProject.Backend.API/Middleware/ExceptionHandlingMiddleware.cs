using System.Text.Json;
using CoreProject.Backend.API.Common.Models;
using CoreProject.Backend.Application.Common.Exceptions;

namespace CoreProject.Backend.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            _logger.LogWarning(exception, "Validation error occurred. TraceId: {TraceId}", context.TraceIdentifier);
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, exception.Message, exception.Errors);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled error occurred. TraceId: {TraceId}", context.TraceIdentifier);
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string message,
        IDictionary<string, string[]>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Status = statusCode,
            Message = message,
            Errors = errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonSerializerOptions));
    }
}
