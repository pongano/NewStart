using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.Configuration.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreProject.Backend.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ConfigurationEntry> ConfigurationEntries => Set<ConfigurationEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
