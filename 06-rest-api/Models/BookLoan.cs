using System;

namespace LibraryApi.Models;

/// <summary>
/// Сутність видачі книги читачеві (зв'язок багато-до-багатьох із додатковими атрибутами).
/// </summary>
public class BookLoan
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book? Book { get; set; }

    public int ReaderId { get; set; }
    public Reader? Reader { get; set; }

    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; } = false;
}
