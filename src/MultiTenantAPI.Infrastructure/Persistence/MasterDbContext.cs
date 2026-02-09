using Microsoft.EntityFrameworkCore;
using MultiTenantAPI.Domain.Entities;

namespace MultiTenantAPI.Infrastructure.Persistence;

public sealed class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.CompanyCode).IsUnique();
            entity.Property(x => x.CompanyCode).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ConnectionString).HasMaxLength(500).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        });
    }
}
