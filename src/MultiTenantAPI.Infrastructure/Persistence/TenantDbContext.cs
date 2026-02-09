using Microsoft.EntityFrameworkCore;
using MultiTenantAPI.Application.Tenants;
using MultiTenantAPI.Domain.Entities;

namespace MultiTenantAPI.Infrastructure.Persistence;

public sealed class TenantDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public TenantDbContext(DbContextOptions<TenantDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var tenant = _tenantProvider.GetCurrentTenant();
        optionsBuilder.UseSqlServer(tenant.ConnectionString, sql =>
        {
            sql.EnableRetryOnFailure();
        });
    }
}
