using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Application.DTOs;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.Interfaces;

namespace LibraryApi.Application.Services;

public interface IBookService
{
    Task<IReadOnlyList<BookDto>> GetAllBooksAsync();
    Task<BookDto> GetBookByIdAsync(int id);
    Task<BookDto> CreateBookAsync(CreateBookDto dto);
    Task UpdateBookAsync(int id, UpdateBookDto dto);
    Task DeleteBookAsync(int id);
}

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<BookDto>> GetAllBooksAsync()
    {
        var books = await _unitOfWork.Books.GetAllAsync();
        return books.Select(MapToDto).ToList();
    }

    public async Task<BookDto> GetBookByIdAsync(int id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
        {
            throw new EntityNotFoundException(nameof(Book), id);
        }

        return MapToDto(book);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        // Бізнес-правило: Перевірка існування автора перед створенням книги
        bool authorExists = await _unitOfWork.Authors.ExistsAsync(dto.AuthorId);
        if (!authorExists)
        {
            throw new BusinessRuleValidationException($"Автора з Id={dto.AuthorId} не знайдено в базі даних.");
        }

        var book = new Book
        {
            Title = dto.Title,
            ISBN = dto.ISBN,
            PublicationYear = dto.PublicationYear,
            Genre = dto.Genre,
            AuthorId = dto.AuthorId
        };

        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        var author = await _unitOfWork.Authors.GetByIdAsync(dto.AuthorId);
        book.Author = author;

        return MapToDto(book);
    }

    public async Task UpdateBookAsync(int id, UpdateBookDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
        {
            throw new EntityNotFoundException(nameof(Book), id);
        }

        bool authorExists = await _unitOfWork.Authors.ExistsAsync(dto.AuthorId);
        if (!authorExists)
        {
            throw new BusinessRuleValidationException($"Автора з Id={dto.AuthorId} не знайдено в системі.");
        }

        book.Title = dto.Title;
        book.ISBN = dto.ISBN;
        book.PublicationYear = dto.PublicationYear;
        book.Genre = dto.Genre;
        book.AuthorId = dto.AuthorId;

        _unitOfWork.Books.Update(book);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
        {
            throw new EntityNotFoundException(nameof(Book), id);
        }

        _unitOfWork.Books.Delete(book);
        await _unitOfWork.SaveChangesAsync();
    }

    private static BookDto MapToDto(Book b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        ISBN = b.ISBN,
        PublicationYear = b.PublicationYear,
        Genre = b.Genre,
        AuthorId = b.AuthorId,
        AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Невідомий автор"
    };
}
