namespace WarehouseApi.Models;

/// <summary>Запит на поповнення (таблиця RestockRequests, "Потреби закупівель").</summary>
public class RestockRequest
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    /// <summary>Залишок на момент створення запиту.</summary>
    public int CurrentQuantity { get; set; }

    public int MinStockLevel { get; set; }

    public DateTime CreatedAt { get; set; }
}
