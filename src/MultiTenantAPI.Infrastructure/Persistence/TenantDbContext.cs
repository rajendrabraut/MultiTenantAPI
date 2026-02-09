using Microsoft.EntityFrameworkCore;
using MultiTenantAPI.Application.Tenants;
using MultiTenantAPI.Domain.Entities;
using MultiTenantAPI.Domain.Inventory;

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
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

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

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.ToTable("InventoryItems");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Sku).IsUnique();
            entity.Property(x => x.Sku).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UnitCost).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ReorderPoint).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.ToTable("StockMovements");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Reason).HasMaxLength(200).IsRequired();
            entity.Property(x => x.OccurredAtUtc).IsRequired();
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouses");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(200).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        });

        SeedInventory(modelBuilder);
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

    private static void SeedInventory(ModelBuilder modelBuilder)
    {
        var electronicsId = Guid.Parse("6c2f77d9-4a58-4b99-95c4-07036c2bdc9f");
        var consumablesId = Guid.Parse("4cf5e51a-1d52-4c27-a235-9b0f8b2919f2");
        var supplierNorthId = Guid.Parse("b10f2b2b-8e3c-4ac8-8d72-0784c38317c1");
        var supplierGlobalId = Guid.Parse("d7a38f5c-6f7b-4f0d-9b09-7a5e4e9b6f7c");
        var warehouseMainId = Guid.Parse("5a1f2e2b-1ccf-4c5a-87ef-6f8f0c7a92b7");
        var itemScannerId = Guid.Parse("1f9a0a58-13b5-4a8e-9f4f-3e1e4f0b0f01");
        var itemGlovesId = Guid.Parse("2e3f4a5b-6c7d-8e9f-1a2b-3c4d5e6f7081");
        var movementSeedId = Guid.Parse("a1b2c3d4-e5f6-47a8-9b0c-1d2e3f4a5b6c");

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = electronicsId, Name = "Electronics", IsActive = true },
            new Category { Id = consumablesId, Name = "Consumables", IsActive = true });

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = supplierNorthId, Name = "Northwind Supply", Email = "orders@northwind.example", IsActive = true },
            new Supplier { Id = supplierGlobalId, Name = "Global Distribution", Email = "sales@global.example", IsActive = true });

        modelBuilder.Entity<Warehouse>().HasData(
            new Warehouse { Id = warehouseMainId, Name = "Main Warehouse", Location = "Dallas, TX", IsActive = true });

        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem
            {
                Id = itemScannerId,
                Sku = "SCN-1000",
                Name = "Handheld Scanner",
                CategoryId = electronicsId,
                SupplierId = supplierNorthId,
                ReorderPoint = 5,
                UnitCost = 249.99m,
                IsActive = true
            },
            new InventoryItem
            {
                Id = itemGlovesId,
                Sku = "GLV-200",
                Name = "Warehouse Gloves",
                CategoryId = consumablesId,
                SupplierId = supplierGlobalId,
                ReorderPoint = 50,
                UnitCost = 2.99m,
                IsActive = true
            });

        modelBuilder.Entity<StockMovement>().HasData(
            new StockMovement
            {
                Id = movementSeedId,
                InventoryItemId = itemGlovesId,
                QuantityDelta = 150,
                Reason = "Initial stock",
                OccurredAtUtc = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}
