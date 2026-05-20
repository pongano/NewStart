namespace CoreProject.Backend.Application.Common.Exceptions;

public sealed class ValidationException : Exception
{
    public ValidationException(string message, IDictionary<string, string[]>? errors = null)
        : base(message)
    {
        Errors = errors;
    }

    public IDictionary<string, string[]>? Errors { get; }
}
