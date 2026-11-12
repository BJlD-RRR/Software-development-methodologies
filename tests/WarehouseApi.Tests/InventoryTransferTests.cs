using Moq;
using WarehouseApi.Dtos;
using WarehouseApi.Exceptions;
using WarehouseApi.Models;
using WarehouseApi.Services;
using Xunit;

namespace WarehouseApi.Tests;

/// <summary>TS-24: недостатньо товару на складі-відправнику.</summary>
public class InventoryTransferTests
{
    private static readonly Product Product = new() { Id = 1, Sku = "ART-123", Name = "Болт М8" };

    [Fact]
    public async Task CheckAvailability_RequestedMoreThanAvailable_ThrowsInsufficientStock()
    {
        var source = new InventoryBalance { ProductId = 1, WarehouseId = 1, Quantity = 3 };
        var repo = InventoryTestHelper.CreateRepository(Product, source);
        var service = new InventoryService(repo.Object);

        await Assert.ThrowsAsync<InsufficientStockException>(() => service.CheckAvailabilityAsync(1, 1, 5));
        Assert.Equal(3, source.Quantity);
    }

    [Fact]
    public async Task Transfer_InsufficientStock_ThrowsAndBalancesUnchanged()
    {
        var source = new InventoryBalance { ProductId = 1, WarehouseId = 1, Quantity = 3 };
        var destination = new InventoryBalance { ProductId = 1, WarehouseId = 2, Quantity = 0 };
        var repo = InventoryTestHelper.CreateRepository(Product, source, destination);
        var service = new InventoryService(repo.Object);

        await Assert.ThrowsAsync<InsufficientStockException>(() => service.TransferAsync(new TransferRequestDto
        {
            ProductId = 1,
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            Quantity = 5
        }));

        Assert.Equal(3, source.Quantity);
        Assert.Equal(0, destination.Quantity);
        repo.Verify(r => r.AddTransaction(It.IsAny<Transaction>()), Times.Never);
        repo.Verify(r => r.AddBalance(It.IsAny<InventoryBalance>()), Times.Never);
    }
}
