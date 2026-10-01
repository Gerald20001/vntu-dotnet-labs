using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}

public class CreateBookDto
{
    [Required(ErrorMessage = "Назва книги є обов'язковою.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Назва книги має містити від 1 до 200 символів.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN є обов'язковим.")]
    [RegularExpression(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$", 
        ErrorMessage = "Некоректний формат ISBN.")]
    public string ISBN { get; set; } = string.Empty;

    [Range(1000, 2030, ErrorMessage = "Рік видання має бути в межах від 1000 до 2030 року.")]
    public int PublicationYear { get; set; }

    [Required(ErrorMessage = "Жанр є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Жанр не може перевищувати 50 символів.")]
    public string Genre { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Необхідно вказати дійсний Id автора (> 0).")]
    public int AuthorId { get; set; }
}

public class UpdateBookDto
{
    [Required(ErrorMessage = "Назва книги є обов'язковою.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Назва книги має містити від 1 до 200 символів.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN є обов'язковим.")]
    public string ISBN { get; set; } = string.Empty;

    [Range(1000, 2030, ErrorMessage = "Рік видання має бути в межах від 1000 до 2030 року.")]
    public int PublicationYear { get; set; }

    [Required(ErrorMessage = "Жанр є обов'язковим.")]
    public string Genre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Необхідно вказати дійсний Id автора (> 0).")]
    public int AuthorId { get; set; }
}
