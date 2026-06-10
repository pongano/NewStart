using CoreProject.Backend.Domain.AccessControl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreProject.Backend.Infrastructure.Persistence.Configurations.AccessControl;

public sealed class MenuPermissionConfiguration : IEntityTypeConfiguration<MenuPermission>
{
    public void Configure(EntityTypeBuilder<MenuPermission> builder)
    {
        builder.ToTable("menu_permissions");

        builder.HasKey(x => new { x.MenuId, x.PermissionId });

        builder.Property(x => x.LinkedBy)
            .HasMaxLength(100);

        builder.HasIndex(x => x.PermissionId);

        builder.HasOne(x => x.Menu)
            .WithMany(x => x.MenuPermissions)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.MenuPermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
