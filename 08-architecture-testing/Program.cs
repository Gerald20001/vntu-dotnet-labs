using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Interfaces;
using LibraryApi.Infrastructure.Data;
using LibraryApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Налаштування рядка підключення SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=library_clean.db";

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(connectionString));

// Реєстрація залежностей Clean Architecture (DI Container)
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Автоматичне створення бази даних та початкових даних (Seed)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    dbContext.Database.EnsureCreated();
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

public partial class Program { }
