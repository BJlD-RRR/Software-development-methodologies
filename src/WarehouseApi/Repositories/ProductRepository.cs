using Microsoft.EntityFrameworkCore;
using WarehouseApi.Data;
using WarehouseApi.Models;

namespace WarehouseApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) => _db = db;

    public Task<bool> SkuExistsAsync(string sku) => _db.Products.AnyAsync(p => p.Sku == sku);

    public Task<Product?> GetByIdAsync(int id) => _db.Products.FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<Product>> GetAllAsync() => _db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }
}
