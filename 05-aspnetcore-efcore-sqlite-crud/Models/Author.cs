using System.Collections.Generic;

namespace LibraryApi.Models;

/// <summary>
/// Сутність автора книги (1-до-багатьох із Book).
/// </summary>
public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Country { get; set; }

    // Навігаційна властивість: автор може мати багато книг
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
