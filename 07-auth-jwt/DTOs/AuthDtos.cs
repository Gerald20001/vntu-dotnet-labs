using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Ім'я користувача є обов'язковим.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username має містити від 3 до 50 символів.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Некоректний формат email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль є обов'язковим.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен містити щонайменше 6 символів.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Роль: "Admin" або "User" (за замовчуванням "User")
    /// </summary>
    public string Role { get; set; } = "User";
}

public class LoginDto
{
    [Required(ErrorMessage = "Вкажіть ім'я користувача.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть пароль.")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
