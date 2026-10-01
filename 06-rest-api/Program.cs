using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using LibraryApi.Data;
using LibraryApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Реєстрація DbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=library_lab06.db"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 2. Глобальна обробка помилок (ProblemDetails RFC 7807)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 3. Ініціалізація бази даних та Seed-даних
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
