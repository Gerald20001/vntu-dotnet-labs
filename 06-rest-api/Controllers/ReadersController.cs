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
public class ReadersController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public ReadersController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReaderDto>>> GetReaders()
    {
        return await _context.Readers
            .AsNoTracking()
            .Select(r => new ReaderDto
            {
                Id = r.Id,
                FullName = r.FullName,
                Email = r.Email,
                TicketNumber = r.TicketNumber,
                PhoneNumber = r.PhoneNumber
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReaderDto>> GetReaderById(int id)
    {
        var reader = await _context.Readers.FindAsync(id);
        if (reader == null)
        {
            return NotFound(new ProblemDetails { Status = 404, Title = "Читача не знайдено", Detail = $"Читач #{id} не зареєстрований." });
        }

        return Ok(new ReaderDto
        {
            Id = reader.Id,
            FullName = reader.FullName,
            Email = reader.Email,
            TicketNumber = reader.TicketNumber,
            PhoneNumber = reader.PhoneNumber
        });
    }

    /// <summary>
    /// Отримати історію видач книг конкретному читачеві.
    /// </summary>
    [HttpGet("{id:int}/loans")]
    public async Task<ActionResult<IEnumerable<BookLoanDto>>> GetLoansByReader(int id)
    {
        var readerExists = await _context.Readers.AnyAsync(r => r.Id == id);
        if (!readerExists)
        {
            return NotFound(new ProblemDetails { Status = 404, Title = "Читача не знайдено", Detail = $"Читач #{id} не знайдений." });
        }

        var loans = await _context.BookLoans
            .Include(l => l.Book)
            .Include(l => l.Reader)
            .Where(l => l.ReaderId == id)
            .AsNoTracking()
            .Select(l => new BookLoanDto
            {
                Id = l.Id,
                BookId = l.BookId,
                BookTitle = l.Book!.Title,
                ReaderId = l.ReaderId,
                ReaderName = l.Reader!.FullName,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                IsReturned = l.IsReturned
            })
            .ToListAsync();

        return Ok(loans);
    }

    [HttpPost]
    public async Task<ActionResult<ReaderDto>> CreateReader([FromBody] CreateReaderDto dto)
    {
        var reader = new Reader
        {
            FullName = dto.FullName,
            Email = dto.Email,
            TicketNumber = dto.TicketNumber,
            PhoneNumber = dto.PhoneNumber
        };

        _context.Readers.Add(reader);
        await _context.SaveChangesAsync();

        var result = new ReaderDto
        {
            Id = reader.Id,
            FullName = reader.FullName,
            Email = reader.Email,
            TicketNumber = reader.TicketNumber,
            PhoneNumber = reader.PhoneNumber
        };

        return CreatedAtAction(nameof(GetReaderById), new { id = reader.Id }, result);
    }
}
