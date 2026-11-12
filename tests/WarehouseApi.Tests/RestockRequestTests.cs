using Moq;
using WarehouseApi.Models;
using WarehouseApi.Services;
using Xunit;

namespace WarehouseApi.Tests;

/// <summary>TS-27: запит на поповнення створюється лише коли залишок &lt;= MinStockLevel.</summary>
public class RestockRequestTests
{
    [Fact]
    public async Task WriteOff_CreatesRestockRequest_OnlyWhenQuantityAtOrBelowMinStockLevel()
    {
        var product = new Product { Id = 1, Sku = "ART-123", Name = "Болт М8", MinStockLevel = 10 };
        var balance = new InventoryBalance { ProductId = 1, WarehouseId = 1, Quantity = 20 };
        var repo = InventoryTestHelper.CreateRepository(product, balance);
        var service = new InventoryService(repo.Object);

        // Списання залишає 11 одиниць — запит НЕ створюється
        await service.WriteOffAsync(1, 1, 9);
        Assert.Equal(11, balance.Quantity);
        repo.Verify(r => r.AddRestockRequest(It.IsAny<RestockRequest>()), Times.Never);

        // Списання залишає 9 одиниць — запит СТВОРЮЄТЬСЯ
        await service.WriteOffAsync(1, 1, 2);
        Assert.Equal(9, balance.Quantity);
        repo.Verify(r => r.AddRestockRequest(It.Is<RestockRequest>(rr =>
            rr.ProductId == 1 && rr.WarehouseId == 1 && rr.CurrentQuantity == 9 && rr.MinStockLevel == 10)), Times.Once);
    }

    [Fact]
    public async Task WriteOff_QuantityEqualsMinStockLevel_CreatesRestockRequest()
    {
        var product = new Product { Id = 1, Sku = "ART-123", Name = "Болт М8", MinStockLevel = 10 };
        var balance = new InventoryBalance { ProductId = 1, WarehouseId = 1, Quantity = 15 };
        var repo = InventoryTestHelper.CreateRepository(product, balance);
        var service = new InventoryService(repo.Object);

        await service.WriteOffAsync(1, 1, 5);

        repo.Verify(r => r.AddRestockRequest(It.Is<RestockRequest>(rr => rr.CurrentQuantity == 10)), Times.Once);
    }
}
