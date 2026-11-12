using Moq;
using WarehouseApi.Dtos;
using WarehouseApi.Models;
using WarehouseApi.Services;
using Xunit;

namespace WarehouseApi.Tests;

/// <summary>TS-20: перевірка правильного збільшення залишку товару на складі.</summary>
public class InventoryReceiptTests
{
    private static readonly Product Product = new() { Id = 1, Sku = "ART-123", Name = "Болт М8" };

    [Fact]
    public async Task Receive_ExistingBalance_IncreasesQuantity()
    {
        var balance = new InventoryBalance { ProductId = 1, WarehouseId = 1, Quantity = 10 };
        var repo = InventoryTestHelper.CreateRepository(Product, balance);
        var service = new InventoryService(repo.Object);

        var result = await service.ReceiveAsync(new ReceiptRequestDto { ProductId = 1, WarehouseId = 1, Quantity = 5 });

        Assert.Equal(15, balance.Quantity);
        Assert.Equal(15, result.Quantity);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Receive_NoBalanceYet_CreatesBalanceWithReceivedQuantity()
    {
        var repo = InventoryTestHelper.CreateRepository(Product);
        var service = new InventoryService(repo.Object);

        var result = await service.ReceiveAsync(new ReceiptRequestDto { ProductId = 1, WarehouseId = 2, Quantity = 7 });

        Assert.Equal(7, result.Quantity);
        repo.Verify(r => r.AddBalance(It.Is<InventoryBalance>(b =>
            b.ProductId == 1 && b.WarehouseId == 2 && b.Quantity == 7)), Times.Once);
    }

    [Fact]
    public async Task Receive_WritesReceiptTransactionToHistory()
    {
        var repo = InventoryTestHelper.CreateRepository(Product,
            new InventoryBalance { ProductId = 1, WarehouseId = 1, Quantity = 0 });
        var service = new InventoryService(repo.Object);

        await service.ReceiveAsync(new ReceiptRequestDto { ProductId = 1, WarehouseId = 1, Quantity = 3 });

        repo.Verify(r => r.AddTransaction(It.Is<Transaction>(t =>
            t.ProductId == 1 && t.WarehouseId == 1 && t.Quantity == 3 && t.Type == TransactionType.Receipt)), Times.Once);
    }
}
