namespace CoreProject.Backend.API.Common.Models;

public sealed class ApiErrorResponse
{
    public required string TraceId { get; init; }
    public required int Status { get; init; }
    public required string Message { get; init; }
    public IDictionary<string, string[]>? Errors { get; init; }
}
