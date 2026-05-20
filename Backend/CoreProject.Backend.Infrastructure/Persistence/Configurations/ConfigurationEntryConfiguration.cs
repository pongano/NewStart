using CoreProject.Backend.Domain.Configuration.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreProject.Backend.Infrastructure.Persistence.Configurations;

public sealed class ConfigurationEntryConfiguration : IEntityTypeConfiguration<ConfigurationEntry>
{
    public void Configure(EntityTypeBuilder<ConfigurationEntry> builder)
    {
        builder.ToTable("configuration_entries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Key)
            .IsUnique();
    }
}
