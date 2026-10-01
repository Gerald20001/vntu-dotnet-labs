using System.Collections.Generic;

namespace LibraryApi.Models;

/// <summary>
/// Сутність читача бібліотеки.
/// </summary>
public class Reader
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
}
