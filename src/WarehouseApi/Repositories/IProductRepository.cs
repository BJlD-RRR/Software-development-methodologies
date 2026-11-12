using WarehouseApi.Models;

namespace WarehouseApi.Repositories;

public interface IProductRepository
{
    Task<bool> SkuExistsAsync(string sku);
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
}
