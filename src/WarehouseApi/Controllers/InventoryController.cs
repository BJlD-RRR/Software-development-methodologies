using Microsoft.AspNetCore.Mvc;
using WarehouseApi.Dtos;
using WarehouseApi.Exceptions;
using WarehouseApi.Services;

namespace WarehouseApi.Controllers;

[ApiController]
[Route("api/inventory")]
[Produces("application/json")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _service;

    public InventoryController(InventoryService service) => _service = service;

    /// <summary>Фіксує надходження партії товару на склад.</summary>
    /// <remarks>
    /// Збільшує залишок товару на обраному складі та записує операцію в журнал рухів.
    ///
    /// Приклад запиту:
    ///
    ///     POST /api/inventory/receipt
    ///     {
    ///         "productId": 1,
    ///         "warehouseId": 1,
    ///         "quantity": 50
    ///     }
    ///
    /// Поля тіла запиту:
    /// - `productId` — ідентифікатор товару (обов'язково, ціле число &gt; 0);
    /// - `warehouseId` — ідентифікатор складу призначення (обов'язково, ціле число &gt; 0);
    /// - `quantity` — кількість товару в партії (обов'язково, ціле число &gt; 0).
    /// </remarks>
    /// <param name="request">Дані надходження.</param>
    /// <response code="200">Надходження зафіксовано, повертається новий залишок.</response>
    /// <response code="400">Некоректні дані, товар або склад не знайдено.</response>
    [HttpPost("receipt")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(BalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Receipt([FromBody] ReceiptRequestDto request)
    {
        try
        {
            return Ok(await _service.ReceiveAsync(request));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Помилка валідації", Detail = ex.Message, Status = 400 });
        }
    }

    /// <summary>Переміщує товар з одного складу на інший.</summary>
    /// <remarks>
    /// Приклад запиту:
    ///
    ///     POST /api/inventory/transfer
    ///     {
    ///         "productId": 1,
    ///         "fromWarehouseId": 1,
    ///         "toWarehouseId": 2,
    ///         "quantity": 5
    ///     }
    ///
    /// </remarks>
    /// <param name="request">Дані переміщення.</param>
    /// <response code="204">Переміщення виконано.</response>
    /// <response code="400">Некоректні дані або недостатньо товару на складі-відправнику.</response>
    [HttpPost("transfer")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request)
    {
        try
        {
            await _service.TransferAsync(request);
            return NoContent();
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Недостатньо товару", Detail = ex.Message, Status = 400 });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Помилка валідації", Detail = ex.Message, Status = 400 });
        }
    }
}
