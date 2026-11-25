using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarehouseApi.Data;
using WarehouseApi.Repositories;
using WarehouseApi.Services;

var builder = WebApplication.CreateBuilder(args);

// TS-29: підключення до бази даних та Entity Framework Core (PostgreSQL).
// У Docker рядок підключення передається змінною середовища ConnectionStrings__DefaultConnection.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<InventoryService>();

builder.Services.AddControllers();

// TS-17, TS-21: документація API у Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
