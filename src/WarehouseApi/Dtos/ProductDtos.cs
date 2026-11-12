using System.ComponentModel.DataAnnotations;

namespace WarehouseApi.Dtos;

/// <summary>Дані для створення нового товару.</summary>
public class CreateProductDto
{
    /// <summary>Артикул товару (унікальний).</summary>
    /// <example>ART-123</example>
    [Required, MaxLength(64)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>Назва товару.</summary>
    /// <example>Болт М8</example>
    [Required, MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Категорія товару.</summary>
    /// <example>Кріплення</example>
    [Required, MaxLength(128)]
    public string Category { get; set; } = string.Empty;

    /// <summary>Одиниця виміру (шт, кг тощо).</summary>
    /// <example>шт</example>
    [Required, MaxLength(32)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>Мінімальний залишок, при досягненні якого створюється запит на поповнення.</summary>
    /// <example>10</example>
    [Range(0, int.MaxValue)]
    public int MinStockLevel { get; set; }
}

/// <summary>Товар.</summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int MinStockLevel { get; set; }
}
