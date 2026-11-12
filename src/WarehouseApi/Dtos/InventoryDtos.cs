using System.ComponentModel.DataAnnotations;

namespace WarehouseApi.Dtos;

/// <summary>Тіло запиту на надходження партії товару на склад.</summary>
public class ReceiptRequestDto
{
    /// <summary>Ідентифікатор товару.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    /// <summary>Ідентифікатор складу призначення.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int WarehouseId { get; set; }

    /// <summary>Кількість товару в партії (більше 0).</summary>
    /// <example>50</example>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

/// <summary>Тіло запиту на переміщення товару між складами.</summary>
public class TransferRequestDto
{
    /// <summary>Ідентифікатор товару.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    /// <summary>Склад-відправник.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int FromWarehouseId { get; set; }

    /// <summary>Склад-одержувач.</summary>
    /// <example>2</example>
    [Range(1, int.MaxValue)]
    public int ToWarehouseId { get; set; }

    /// <summary>Кількість товару (більше 0).</summary>
    /// <example>5</example>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

/// <summary>Залишок товару на складі.</summary>
public class BalanceDto
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
}
