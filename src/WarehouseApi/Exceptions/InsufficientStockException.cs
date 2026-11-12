namespace WarehouseApi.Exceptions;

/// <summary>Недостатньо товару на складі для виконання операції.</summary>
public class InsufficientStockException : Exception
{
    public InsufficientStockException(int productId, int warehouseId, int requested, int available)
        : base($"Недостатньо товару (Id={productId}) на складі (Id={warehouseId}): запитано {requested}, доступно {available}.")
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        Requested = requested;
        Available = available;
    }

    public int ProductId { get; }
    public int WarehouseId { get; }
    public int Requested { get; }
    public int Available { get; }
}
