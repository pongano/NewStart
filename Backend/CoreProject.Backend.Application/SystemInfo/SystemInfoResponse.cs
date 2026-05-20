namespace CoreProject.Backend.Application.SystemInfo;

public sealed class SystemInfoResponse
{
    public string ServiceName { get; init; } = string.Empty;
    public string Environment { get; init; } = string.Empty;
    public DateTime ServerTimeUtc { get; init; }
    public string Version { get; init; } = string.Empty;
}
