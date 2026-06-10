using CoreProject.Backend.Domain.Audit.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreProject.Backend.Infrastructure.Persistence.Configurations.Audit;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .HasMaxLength(AuditLog.ActionMaxLength)
            .IsRequired();

        builder.Property(x => x.EntityType)
            .HasMaxLength(AuditLog.EntityTypeMaxLength)
            .IsRequired();

        builder.Property(x => x.EntityId)
            .HasMaxLength(AuditLog.EntityIdMaxLength);

        builder.Property(x => x.Method)
            .HasMaxLength(AuditLog.MethodMaxLength)
            .IsRequired();

        builder.Property(x => x.Path)
            .HasMaxLength(AuditLog.PathMaxLength)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(AuditLog.UserIdMaxLength);

        builder.Property(x => x.TraceId)
            .HasMaxLength(AuditLog.TraceIdMaxLength);

        builder.Property(x => x.Details)
            .HasMaxLength(AuditLog.DetailsMaxLength);

        builder.HasIndex(x => x.OccurredAtUtc);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.UserId);
    }
}
