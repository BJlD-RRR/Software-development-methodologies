using Microsoft.EntityFrameworkCore;
using WarehouseApi.Models;

namespace WarehouseApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<RestockRequest> RestockRequests => Set<RestockRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.Sku).IsUnique();
            e.Property(p => p.Sku).IsRequired().HasMaxLength(64);
            e.Property(p => p.Name).IsRequired().HasMaxLength(256);
            e.Property(p => p.Category).IsRequired().HasMaxLength(128);
            e.Property(p => p.UnitOfMeasure).IsRequired().HasMaxLength(32);
        });

        modelBuilder.Entity<Warehouse>(e =>
        {
            e.Property(w => w.Name).IsRequired().HasMaxLength(256);
            // Склади, з якими працюють операції надходження та переміщення.
            e.HasData(
                new Warehouse { Id = 1, Name = "Основний склад" },
                new Warehouse { Id = 2, Name = "Резервний склад" });
        });

        modelBuilder.Entity<InventoryBalance>(e =>
        {
            e.HasIndex(b => new { b.ProductId, b.WarehouseId }).IsUnique();
        });
    }
}
