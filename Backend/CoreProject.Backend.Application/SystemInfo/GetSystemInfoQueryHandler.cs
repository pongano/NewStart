using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.SystemInfo;

public sealed class GetSystemInfoQueryHandler
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetSystemInfoQueryHandler(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task<SystemInfoResponse> HandleAsync(
        GetSystemInfoQuery query,
        string serviceName,
        string environmentName,
        string version,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var response = new SystemInfoResponse
        {
            ServiceName = serviceName,
            Environment = environmentName,
            ServerTimeUtc = _dateTimeProvider.UtcNow,
            Version = version
        };

        return Task.FromResult(response);
    }
}
