using System.Collections.Generic;
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
public class AuthorsController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public AuthorsController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        return await _context.Authors
            .AsNoTracking()
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Country = a.Country,
                BooksCount = a.Books.Count
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDto>> GetAuthorById(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return NotFound(new ProblemDetails { Status = 404, Title = "Автора не знайдено", Detail = $"Автор #{id} відсутній." });
        }

        return Ok(new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            Country = author.Country,
            BooksCount = author.Books.Count
        });
    }

    /// <summary>
    /// Отримати всі книги вказаного автора (робота з пов'язаними сутностями).
    /// </summary>
    [HttpGet("{id:int}/books")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByAuthor(int id)
    {
        var authorExists = await _context.Authors.AnyAsync(a => a.Id == id);
        if (!authorExists)
        {
            return NotFound(new ProblemDetails { Status = 404, Title = "Автора не знайдено", Detail = $"Автор #{id} відсутній." });
        }

        var books = await _context.Books
            .Where(b => b.AuthorId == id)
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

        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorDto dto)
    {
        var author = new Author
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Country = dto.Country
        };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        var result = new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            Country = author.Country,
            BooksCount = 0
        };

        return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, result);
    }
}
