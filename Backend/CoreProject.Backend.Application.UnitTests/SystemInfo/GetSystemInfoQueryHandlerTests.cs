using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Application.SystemInfo;

namespace CoreProject.Backend.Application.UnitTests.SystemInfo;

public sealed class GetSystemInfoQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnExpectedSystemInfo()
    {
        var utcNow = new DateTime(2026, 5, 21, 0, 0, 0, DateTimeKind.Utc);
        var handler = new GetSystemInfoQueryHandler(new FakeDateTimeProvider(utcNow));

        var response = await handler.HandleAsync(
            new GetSystemInfoQuery(),
            serviceName: "CoreProject.Backend.API",
            environmentName: "Development",
            version: "1.0.0");

        Assert.Equal("CoreProject.Backend.API", response.ServiceName);
        Assert.Equal("Development", response.Environment);
        Assert.Equal("1.0.0", response.Version);
        Assert.Equal(utcNow, response.ServerTimeUtc);
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}
