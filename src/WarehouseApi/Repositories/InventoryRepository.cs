using Microsoft.EntityFrameworkCore;
using WarehouseApi.Data;
using WarehouseApi.Models;

namespace WarehouseApi.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _db;

    public InventoryRepository(ApplicationDbContext db) => _db = db;

    public Task<Product?> GetProductAsync(int productId) =>
        _db.Products.FirstOrDefaultAsync(p => p.Id == productId);

    public Task<bool> WarehouseExistsAsync(int warehouseId) =>
        _db.Warehouses.AnyAsync(w => w.Id == warehouseId);

    public Task<InventoryBalance?> GetBalanceAsync(int productId, int warehouseId) =>
        _db.InventoryBalances.FirstOrDefaultAsync(b => b.ProductId == productId && b.WarehouseId == warehouseId);

    public void AddBalance(InventoryBalance balance) => _db.InventoryBalances.Add(balance);

    public void AddTransaction(Transaction transaction) => _db.Transactions.Add(transaction);

    public void AddRestockRequest(RestockRequest request) => _db.RestockRequests.Add(request);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            await action();
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            _db.ChangeTracker.Clear();
            throw;
        }
    }
}
