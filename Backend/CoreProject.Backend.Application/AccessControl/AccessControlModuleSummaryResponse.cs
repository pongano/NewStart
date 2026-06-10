namespace CoreProject.Backend.Application.AccessControl;

public sealed class AccessControlModuleSummaryResponse
{
    public string ModuleName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IReadOnlyCollection<string> PlannedCapabilities { get; set; } = Array.Empty<string>();

    public IReadOnlyCollection<string> PlannedEntities { get; set; } = Array.Empty<string>();
}
