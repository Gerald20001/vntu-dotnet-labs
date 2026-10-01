using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs;

public class ReaderDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public class CreateReaderDto
{
    [Required(ErrorMessage = "ПІБ читача є обов'язковим.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "ПІБ має містити від 3 до 100 символів.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Некоректний формат електронної пошти.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Номер читацького квитка є обов'язковим.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Номер квитка має містити від 3 до 20 символів.")]
    public string TicketNumber { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Некоректний номер телефону.")]
    public string? PhoneNumber { get; set; }
}

public class BookLoanDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int ReaderId { get; set; }
    public string ReaderName { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
}
