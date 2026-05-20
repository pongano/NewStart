using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Infrastructure.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
