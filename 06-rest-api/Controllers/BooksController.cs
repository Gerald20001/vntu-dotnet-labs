using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.DTOs;
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
    /// Отримати пагінований список книг із можливістю фільтрації та сортування.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<BookDto>>> GetBooks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? genre = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = "id",
        [FromQuery] string? sortOrder = "asc")
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        IQueryable<Book> query = _context.Books.Include(b => b.Author).AsNoTracking();

        // 1. Фільтрація
        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(b => b.Genre.ToLower() == genre.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(term) ||
                                     b.Author!.LastName.ToLower().Contains(term));
        }

        int totalItems = await query.CountAsync();

        // 2. Сортування
        bool isDesc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        query = (sortBy?.ToLower()) switch
        {
            "title" => isDesc ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "year" => isDesc ? query.OrderByDescending(b => b.PublicationYear) : query.OrderBy(b => b.PublicationYear),
            _ => isDesc ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id)
        };

        // 3. Пагінація
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                Genre = b.Genre,
                AuthorId = b.AuthorId,
                AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Невідомий"
            })
            .ToListAsync();

        return Ok(new PagedResult<BookDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = totalItems
        });
    }

    /// <summary>
    /// Отримати деталі книги за Id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetBookById(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Книгу не знайдено",
                Detail = $"Книгу з ідентифікатором {id} не знайдено в каталозі бібліотеки."
            });
        }

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
    /// Створити нову книгу (з валідацією вхідного DTO).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto dto)
    {
        var author = await _context.Authors.FindAsync(dto.AuthorId);
        if (author == null)
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Некоректні дані",
                Detail = $"Вказаного автора з Id={dto.AuthorId} не існує в системі."
            });
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

        var resultDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            PublicationYear = book.PublicationYear,
            Genre = book.Genre,
            AuthorId = book.AuthorId,
            AuthorName = $"{author.FirstName} {author.LastName}"
        };

        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, resultDto);
    }

    /// <summary>
    /// Оновити інформацію про книгу.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Книгу не знайдено",
                Detail = $"Книгу з Id={id} не знайдено для оновлення."
            });
        }

        var authorExists = await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId);
        if (!authorExists)
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Некоректний автор",
                Detail = $"Автора з Id={dto.AuthorId} не існує."
            });
        }

        book.Title = dto.Title;
        book.ISBN = dto.ISBN;
        book.PublicationYear = dto.PublicationYear;
        book.Genre = dto.Genre;
        book.AuthorId = dto.AuthorId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Видалити книгу з каталогу.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "Книгу не знайдено",
                Detail = $"Книгу з Id={id} не знайдено для видалення."
            });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
