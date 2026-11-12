using WarehouseApi.Dtos;
using WarehouseApi.Exceptions;
using WarehouseApi.Models;
using WarehouseApi.Repositories;

namespace WarehouseApi.Services;

public class InventoryService
{
    private readonly IInventoryRepository _repository;

    public InventoryService(IInventoryRepository repository) => _repository = repository;

    /// <summary>TS-19: надходження партії товару на склад.</summary>
    public async Task<BalanceDto> ReceiveAsync(ReceiptRequestDto request)
    {
        if (request.Quantity <= 0)
        {
            throw new ValidationException("Кількість має бути більшою за 0.");
        }

        await EnsureProductExistsAsync(request.ProductId);
        await EnsureWarehouseExistsAsync(request.WarehouseId);

        var balance = await IncreaseAsync(request.ProductId, request.WarehouseId, request.Quantity);
        await _repository.SaveChangesAsync();

        return ToDto(balance);
    }

    /// <summary>
    /// TS-22: перевіряє, що на складі достатньо товару.
    /// Кидає <see cref="InsufficientStockException"/>, якщо запитана кількість перевищує залишок.
    /// </summary>
    public async Task CheckAvailabilityAsync(int productId, int warehouseId, int quantity)
    {
        var balance = await _repository.GetBalanceAsync(productId, warehouseId);
        var available = balance?.Quantity ?? 0;
        if (quantity > available)
        {
            throw new InsufficientStockException(productId, warehouseId, quantity, available);
        }
    }

    /// <summary>TS-23: транзакційне переміщення товару між складами.</summary>
    public async Task TransferAsync(TransferRequestDto request)
    {
        if (request.Quantity <= 0)
        {
            throw new ValidationException("Кількість має бути більшою за 0.");
        }

        if (request.FromWarehouseId == request.ToWarehouseId)
        {
            throw new ValidationException("Склад-відправник і склад-одержувач мають відрізнятися.");
        }

        await EnsureProductExistsAsync(request.ProductId);
        await EnsureWarehouseExistsAsync(request.FromWarehouseId);
        await EnsureWarehouseExistsAsync(request.ToWarehouseId);

        await _repository.ExecuteInTransactionAsync(async () =>
        {
            // 1) Перевірка залишку на складі-відправнику
            await CheckAvailabilityAsync(request.ProductId, request.FromWarehouseId, request.Quantity);
            // 2) Списання зі складу-відправника
            await WriteOffAsync(request.ProductId, request.FromWarehouseId, request.Quantity);
            // 3) Оприбуткування на склад-одержувач
            await IncreaseAsync(request.ProductId, request.ToWarehouseId, request.Quantity);
        });
    }

    /// <summary>
    /// Списання товару зі складу (без збереження змін).
    /// TS-26: якщо новий залишок &lt;= MinStockLevel, створюється запит на поповнення.
    /// </summary>
    public async Task WriteOffAsync(int productId, int warehouseId, int quantity)
    {
        await CheckAvailabilityAsync(productId, warehouseId, quantity);

        var balance = (await _repository.GetBalanceAsync(productId, warehouseId))!;
        balance.Quantity -= quantity;

        _repository.AddTransaction(new Transaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            Type = TransactionType.WriteOff,
            CreatedAt = DateTime.UtcNow
        });

        var product = await _repository.GetProductAsync(productId);
        if (product is not null && balance.Quantity <= product.MinStockLevel)
        {
            _repository.AddRestockRequest(new RestockRequest
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                CurrentQuantity = balance.Quantity,
                MinStockLevel = product.MinStockLevel,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task<InventoryBalance> IncreaseAsync(int productId, int warehouseId, int quantity)
    {
        var balance = await _repository.GetBalanceAsync(productId, warehouseId);
        if (balance is null)
        {
            balance = new InventoryBalance { ProductId = productId, WarehouseId = warehouseId, Quantity = 0 };
            _repository.AddBalance(balance);
        }

        balance.Quantity += quantity;

        _repository.AddTransaction(new Transaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            Type = TransactionType.Receipt,
            CreatedAt = DateTime.UtcNow
        });

        return balance;
    }

    private async Task EnsureProductExistsAsync(int productId)
    {
        if (await _repository.GetProductAsync(productId) is null)
        {
            throw new ValidationException($"Товар з Id={productId} не знайдено.");
        }
    }

    private async Task EnsureWarehouseExistsAsync(int warehouseId)
    {
        if (!await _repository.WarehouseExistsAsync(warehouseId))
        {
            throw new ValidationException($"Склад з Id={warehouseId} не знайдено.");
        }
    }

    private static BalanceDto ToDto(InventoryBalance b) => new()
    {
        ProductId = b.ProductId,
        WarehouseId = b.WarehouseId,
        Quantity = b.Quantity
    };
}
