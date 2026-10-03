using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using LibraryApi.Services;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LibraryDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwtService;

    public AuthController(LibraryDbContext context, IPasswordHasher hasher, IJwtTokenService jwtService)
    {
        _context = context;
        _hasher = hasher;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Реєстрація нового користувача в системі.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower()))
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Конфлікт реєстрації",
                Detail = $"Користувач з іменем '{dto.Username}' вже існує."
            });
        }

        string role = dto.Role.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _hasher.HashPassword(dto.Password),
            Role = role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var (token, expiration) = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Expiration = expiration,
            Username = user.Username,
            Role = user.Role
        });
    }

    /// <summary>
    /// Вхід користувача та отримання JWT-токена.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());
        if (user == null || !_hasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new ProblemDetails
            {
                Status = 401,
                Title = "Помилка автентифікації",
                Detail = "Невірне ім'я користувача або пароль."
            });
        }

        var (token, expiration) = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Expiration = expiration,
            Username = user.Username,
            Role = user.Role
        });
    }

    /// <summary>
    /// Перевірка поточного користувача за отриманим токеном.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            Username = User.Identity?.Name,
            Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
            IsAuthenticated = User.Identity?.IsAuthenticated
        });
    }
}
