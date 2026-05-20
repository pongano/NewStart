using CoreProject.Backend.Domain.Common.Entities;

namespace CoreProject.Backend.Domain.Configuration.Entities;

public sealed class ConfigurationEntry : AuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
}
