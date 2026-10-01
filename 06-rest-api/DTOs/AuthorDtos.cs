using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class AuthorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Country { get; set; }
    public int BooksCount { get; set; }
}

public class CreateAuthorDto
{
    [Required(ErrorMessage = "Ім'я автора є обов'язковим.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я має містити від 2 до 50 символів.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Прізвище автора є обов'язковим.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Прізвище має містити від 2 до 50 символів.")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Країна не може перевищувати 50 символів.")]
    public string? Country { get; set; }
}
