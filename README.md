# Warehouse API

REST API для складського обліку: створення товарів, надходження на склад, переміщення між складами та автоматичні запити на поповнення, коли залишок падає до мінімуму.

Проєкт виконано в межах епіка **TS-5** у Jira (лабораторна робота з методологій розробки ПЗ).

## Технології

- .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 + SQLite
- Swagger (Swashbuckle) з XML-документацією
- xUnit + Moq для модульних тестів

## Структура

```
WarehouseManagement.sln
src/WarehouseApi/
  Controllers/    ProductsController, InventoryController
  Services/       ProductService, InventoryService — бізнес-логіка
  Repositories/   доступ до даних (інтерфейси для підміни в тестах)
  Models/         Product, Warehouse, InventoryBalance, Transaction, RestockRequest
  Dtos/           моделі запитів і відповідей API
  Exceptions/     InsufficientStockException, ValidationException
  Data/           ApplicationDbContext
  Migrations/     міграції EF Core
tests/WarehouseApi.Tests/   модульні тести
```

## Запуск

Потрібен [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet run --project src/WarehouseApi --environment Development
```

- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger (доступний лише в середовищі `Development`)

База даних SQLite (`warehouse.db`) створюється автоматично під час першого запуску, міграції застосовуються самі. Рядок підключення задається в `src/WarehouseApi/appsettings.json` (`ConnectionStrings:DefaultConnection`).

У базу одразу додаються два склади: **1 — «Основний склад»** і **2 — «Резервний склад»**.

## Тести

```bash
dotnet test
```

| Тест | Що перевіряє |
|---|---|
| `ProductSkuUniquenessTests` | повторний артикул → `400 Bad Request`, новий → `201 Created` |
| `InventoryReceiptTests` | надходження збільшує залишок і пише запис у журнал рухів |
| `InventoryTransferTests` | при нестачі товару — `InsufficientStockException`, баланси не змінюються |
| `RestockRequestTests` | запит на поповнення створюється лише коли залишок ≤ `MinStockLevel` |

## API

### Товари

| Метод | Шлях | Опис |
|---|---|---|
| `POST` | `/api/products` | створити товар (артикул має бути унікальним) |
| `GET` | `/api/products` | список усіх товарів |
| `GET` | `/api/products/{id}` | товар за ідентифікатором |

```json
POST /api/products
{
  "sku": "ART-123",
  "name": "Болт М8",
  "category": "Кріплення",
  "unitOfMeasure": "шт",
  "minStockLevel": 10
}
```

Відповіді: `201 Created` — товар створено; `400 Bad Request` — некоректні дані або артикул уже існує.

### Залишки

| Метод | Шлях | Опис |
|---|---|---|
| `POST` | `/api/inventory/receipt` | надходження партії товару на склад |
| `POST` | `/api/inventory/transfer` | переміщення товару між складами |

```json
POST /api/inventory/receipt
{
  "productId": 1,
  "warehouseId": 1,
  "quantity": 50
}
```

Відповіді: `200 OK` з новим залишком; `400 Bad Request` — некоректні дані, товар або склад не знайдено.

```json
POST /api/inventory/transfer
{
  "productId": 1,
  "fromWarehouseId": 1,
  "toWarehouseId": 2,
  "quantity": 5
}
```

Відповіді: `204 No Content` — переміщення виконано; `400 Bad Request` — некоректні дані або недостатньо товару на складі-відправнику.

## Бізнес-правила

- **Надходження** збільшує залишок товару на складі й записує операцію в журнал `Transactions`.
- **Переміщення** виконується в одній транзакції БД: перевірка залишку → списання зі складу-відправника → оприбуткування на склад-одержувач. У журнал пишуться два записи (списання і надходження). Якщо товару не вистачає, нічого не змінюється.
- **Запит на поповнення**: якщо після списання залишок стає меншим або рівним `MinStockLevel` товару, у таблицю `RestockRequests` додається запис.

## Міграції

Міграції вже є в репозиторії. Щоб створити нову після зміни моделі:

```bash
dotnet tool restore
dotnet ef migrations add <НазваМіграції> --project src/WarehouseApi
```

`dotnet tool restore` встановлює `dotnet-ef` потрібної версії з `.config/dotnet-tools.json`.

| Міграція | Задача | Що робить |
|---|---|---|
| `CreateProducts` | TS-14 | таблиця `Products` з унікальним індексом на `Sku` |
| `CreateWarehousesBalancesTransactions` | TS-18 | таблиці `Warehouses`, `InventoryBalances`, `Transactions` |
| `AddMinStockLevelAndRestockRequests` | TS-25 | колонка `MinStockLevel` і таблиця `RestockRequests` |

## Відповідність задачам Jira

| Історія | Підзадачі |
|---|---|
| TS-10 Створення та управління товарами | TS-14, TS-15, TS-16, TS-17 |
| TS-11 Облік надходження товарів | TS-18, TS-19, TS-20, TS-21 |
| TS-12 Переміщення між складами | TS-22, TS-23, TS-24 |
| TS-13 Автоматичний запит на поповнення | TS-25, TS-26, TS-27 |
| TS-29 Налаштування підключення до БД та EF | — |
