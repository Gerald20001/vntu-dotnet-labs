using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public BooksController(LibraryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Отримати список усіх книг (разом із даними про автора).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
    {
        return await _context.Books
            .Include(b => b.Author)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Отримати книгу за її ідентифікатором.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound(new { message = $"Книгу з Id={id} не знайдено." });
        }

        return Ok(book);
    }

    /// <summary>
    /// Створити нову книгу в базі даних.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        // Перевіряємо існування вказаного автора
        var authorExists = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
        if (!authorExists)
        {
            return BadRequest(new { message = $"Автора з Id={book.AuthorId} не існує в базі." });
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    /// <summary>
    /// Оновити наявну книгу за ідентифікатором.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
    {
        if (id != updatedBook.Id)
        {
            return BadRequest(new { message = "Id у запиті не збігається з Id у тілі книги." });
        }

        var existingBook = await _context.Books.FindAsync(id);
        if (existingBook == null)
        {
            return NotFound(new { message = $"Книгу з Id={id} не знайдено." });
        }

        existingBook.Title = updatedBook.Title;
        existingBook.ISBN = updatedBook.ISBN;
        existingBook.PublicationYear = updatedBook.PublicationYear;
        existingBook.Genre = updatedBook.Genre;
        existingBook.AuthorId = updatedBook.AuthorId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Видалити книгу за ідентифікатором.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new { message = $"Книгу з Id={id} не знайдено." });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
