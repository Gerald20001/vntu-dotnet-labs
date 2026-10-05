using System;
using System.Collections.Generic;

namespace LibraryApi.Domain.Entities;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Country { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Genre { get; set; } = string.Empty;

    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
}

public class Reader
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;

    public ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
}

public class BookLoan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public int ReaderId { get; set; }
    public Reader? Reader { get; set; }
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    public bool IsReturned { get; set; } = false;
}
