using WarehouseApi.Dtos;
using WarehouseApi.Exceptions;
using WarehouseApi.Models;
using WarehouseApi.Repositories;

namespace WarehouseApi.Services;

public class ProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products) => _products = products;

    /// <summary>Створює товар. Кидає <see cref="ValidationException"/>, якщо артикул уже існує.</summary>
    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var sku = dto.Sku.Trim();
        if (await _products.SkuExistsAsync(sku))
        {
            throw new ValidationException($"Товар з артикулом '{sku}' вже існує.");
        }

        var product = new Product
        {
            Sku = sku,
            Name = dto.Name.Trim(),
            Category = dto.Category.Trim(),
            UnitOfMeasure = dto.UnitOfMeasure.Trim(),
            MinStockLevel = dto.MinStockLevel
        };

        await _products.AddAsync(product);
        return ToDto(product);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _products.GetByIdAsync(id);
        return product is null ? null : ToDto(product);
    }

    public async Task<List<ProductDto>> GetAllAsync() =>
        (await _products.GetAllAsync()).Select(ToDto).ToList();

    private static ProductDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Sku = p.Sku,
        Name = p.Name,
        Category = p.Category,
        UnitOfMeasure = p.UnitOfMeasure,
        MinStockLevel = p.MinStockLevel
    };
}
