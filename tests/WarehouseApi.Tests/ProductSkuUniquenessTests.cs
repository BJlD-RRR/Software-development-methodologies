using Microsoft.AspNetCore.Mvc;
using Moq;
using WarehouseApi.Controllers;
using WarehouseApi.Dtos;
using WarehouseApi.Models;
using WarehouseApi.Repositories;
using WarehouseApi.Services;
using Xunit;

namespace WarehouseApi.Tests;

/// <summary>TS-16: валідація унікальності артикулу.</summary>
public class ProductSkuUniquenessTests
{
    private static CreateProductDto NewDto(string sku) => new()
    {
        Sku = sku,
        Name = "Болт М8",
        Category = "Кріплення",
        UnitOfMeasure = "шт"
    };

    [Fact]
    public async Task Create_WithExistingSku_ReturnsBadRequest()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.SkuExistsAsync("ART-123")).ReturnsAsync(true);
        var controller = new ProductsController(new ProductService(repo.Object));

        var result = await controller.Create(NewDto("ART-123"));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
        repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithNewSku_ReturnsCreated()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.SkuExistsAsync("ART-456")).ReturnsAsync(false);
        var controller = new ProductsController(new ProductService(repo.Object));

        var result = await controller.Create(NewDto("ART-456"));

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        repo.Verify(r => r.AddAsync(It.Is<Product>(p => p.Sku == "ART-456")), Times.Once);
    }
}
