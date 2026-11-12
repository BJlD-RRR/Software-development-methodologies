namespace WarehouseApi.Models;

public enum TransactionType
{
    /// <summary>Надходження.</summary>
    Receipt = 0,

    /// <summary>Списання.</summary>
    WriteOff = 1
}

/// <summary>Запис журналу рухів товару (таблиця Transactions).</summary>
public class Transaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int Quantity { get; set; }

    public TransactionType Type { get; set; }

    public DateTime CreatedAt { get; set; }
}
