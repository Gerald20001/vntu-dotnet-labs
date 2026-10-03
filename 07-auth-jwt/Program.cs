using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using LibraryApi.Data;
using LibraryApi.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Контекст бази даних SQLite
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=library_lab07.db"));

// 2. Реєстрація сервісів хешування паролів та генерації JWT
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// 3. Налаштування схеми автентифікації JwtBearer
string secretKey = builder.Configuration["Jwt:SecretKey"] ?? "SuperSecretKeyForVntuDotNetLab07ShouldBeLongEnough32Bytes!";
string issuer = builder.Configuration["Jwt:Issuer"] ?? "VntuLibraryApi";
string audience = builder.Configuration["Jwt:Audience"] ?? "VntuLibraryClients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 4. Ініціалізація бази даних та додавання початкових адміністраторів
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new LibraryApi.Models.User
            {
                Username = "admin",
                Email = "admin@vntu.edu.ua",
                PasswordHash = hasher.HashPassword("AdminPass123!"),
                Role = "Admin"
            },
            new LibraryApi.Models.User
            {
                Username = "student",
                Email = "student@vntu.edu.ua",
                PasswordHash = hasher.HashPassword("StudentPass123!"),
                Role = "User"
            }
        );
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// 5. Обов'язковий порядок: спочатку автентифікація, потім авторизація
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
