using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using LibraryApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Реєстрація DbContext з SQLite через Scoped DI
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=library_lab05.db"));

// 2. Реєстрація контролерів з ігноруванням циклічних посилань у JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 3. Підключення OpenAPI (.NET 9/10 стандартний стек)
builder.Services.AddOpenApi();

var app = builder.Build();

// 4. Автоматична ініціалізація бази даних та Seed-даних при старті
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.EnsureCreated();
}

// 5. Конфігурація конвеєра HTTP-запитів
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Доступно за адресою /scalar/v1
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
