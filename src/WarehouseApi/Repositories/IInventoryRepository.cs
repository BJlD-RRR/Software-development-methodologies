using WarehouseApi.Models;

namespace WarehouseApi.Repositories;

public interface IInventoryRepository
{
    Task<Product?> GetProductAsync(int productId);
    Task<bool> WarehouseExistsAsync(int warehouseId);

    /// <summary>Повертає залишок товару на складі або null, якщо запису ще немає.</summary>
    Task<InventoryBalance?> GetBalanceAsync(int productId, int warehouseId);

    void AddBalance(InventoryBalance balance);
    void AddTransaction(Transaction transaction);
    void AddRestockRequest(RestockRequest request);

    Task SaveChangesAsync();

    /// <summary>Виконує дію в межах однієї транзакції БД.</summary>
    Task ExecuteInTransactionAsync(Func<Task> action);
}
