namespace WarehouseApi.Exceptions;

/// <summary>Помилка бізнес-валідації (повертається клієнту як 400 Bad Request).</summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
