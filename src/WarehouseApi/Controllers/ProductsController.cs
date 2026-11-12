using Microsoft.AspNetCore.Mvc;
using WarehouseApi.Dtos;
using WarehouseApi.Exceptions;
using WarehouseApi.Services;

namespace WarehouseApi.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service) => _service = service;

    /// <summary>Створює новий товар.</summary>
    /// <remarks>
    /// Приклад запиту:
    ///
    ///     POST /api/products
    ///     {
    ///         "sku": "ART-123",
    ///         "name": "Болт М8",
    ///         "category": "Кріплення",
    ///         "unitOfMeasure": "шт",
    ///         "minStockLevel": 10
    ///     }
    ///
    /// </remarks>
    /// <param name="dto">Дані нового товару.</param>
    /// <response code="201">Товар створено.</response>
    /// <response code="400">Некоректні дані або товар з таким артикулом уже існує.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Помилка валідації", Detail = ex.Message, Status = 400 });
        }
    }

    /// <summary>Повертає список номенклатури (усіх товарів).</summary>
    /// <response code="200">Список товарів.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    /// <summary>Повертає товар за ідентифікатором.</summary>
    /// <response code="200">Товар знайдено.</response>
    /// <response code="404">Товар не знайдено.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }
}
