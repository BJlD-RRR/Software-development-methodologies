using Moq;
using WarehouseApi.Models;
using WarehouseApi.Repositories;

namespace WarehouseApi.Tests;

/// <summary>Налаштовує mock-репозиторій залишків з набором балансів у пам'яті.</summary>
internal static class InventoryTestHelper
{
    public static Mock<IInventoryRepository> CreateRepository(Product product, params InventoryBalance[] balances)
    {
        var store = balances.ToList();
        var repo = new Mock<IInventoryRepository>();

        repo.Setup(r => r.GetProductAsync(product.Id)).ReturnsAsync(product);
        repo.Setup(r => r.WarehouseExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        repo.Setup(r => r.GetBalanceAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((int p, int w) => store.FirstOrDefault(b => b.ProductId == p && b.WarehouseId == w));
        repo.Setup(r => r.AddBalance(It.IsAny<InventoryBalance>()))
            .Callback<InventoryBalance>(store.Add);
        repo.Setup(r => r.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(action => action());

        return repo;
    }
}
