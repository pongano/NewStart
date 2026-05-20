namespace CoreProject.Backend.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
