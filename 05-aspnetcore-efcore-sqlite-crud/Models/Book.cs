namespace LibraryApi.Models;

/// <summary>
/// Сутність книги в бібліотеці.
/// </summary>
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Genre { get; set; } = string.Empty;

    // Зовнішній ключ та навігаційна властивість зв'язку з автором
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}
