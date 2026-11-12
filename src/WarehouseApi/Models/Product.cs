namespace WarehouseApi.Models;

/// <summary>Товар (таблиця Products).</summary>
public class Product
{
    public int Id { get; set; }

    /// <summary>Артикул (унікальний).</summary>
    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    /// <summary>Одиниця виміру (шт, кг тощо).</summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>Мінімальний залишок, при досягненні якого створюється запит на поповнення.</summary>
    public int MinStockLevel { get; set; }
}
