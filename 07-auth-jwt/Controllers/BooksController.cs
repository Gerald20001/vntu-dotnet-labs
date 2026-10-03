using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // За замовчуванням усі ендпоінти вимагають наявності валідного JWT
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public BooksController(LibraryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Отримати список книг (доступно для будь-якого автентифікованого користувача з роллю User або Admin).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        return await _context.Books
            .Include(b => b.Author)
            .AsNoTracking()
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                Genre = b.Genre,
                AuthorId = b.AuthorId,
                AuthorName = $"{b.Author!.FirstName} {b.Author!.LastName}"
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<BookDto>> GetBookById(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound(new { message = $"Книгу #{id} не знайдено." });

        return Ok(new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            PublicationYear = book.PublicationYear,
            Genre = book.Genre,
            AuthorId = book.AuthorId,
            AuthorName = $"{book.Author?.FirstName} {book.Author?.LastName}"
        });
    }

    /// <summary>
    /// Додати нову книгу (лише для користувачів з роллю Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto dto)
    {
        var authorExists = await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId);
        if (!authorExists)
        {
            return BadRequest(new { message = $"Автора #{dto.AuthorId} не існує." });
        }

        var book = new Book
        {
            Title = dto.Title,
            ISBN = dto.ISBN,
            PublicationYear = dto.PublicationYear,
            Genre = dto.Genre,
            AuthorId = dto.AuthorId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            PublicationYear = book.PublicationYear,
            Genre = book.Genre,
            AuthorId = book.AuthorId
        });
    }

    /// <summary>
    /// Видалити книгу (лише для користувачів з роллю Admin).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound(new { message = $"Книгу #{id} не знайдено." });

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
