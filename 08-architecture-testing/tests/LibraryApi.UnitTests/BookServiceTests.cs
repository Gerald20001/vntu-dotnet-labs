using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using LibraryApi.Application.DTOs;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.Interfaces;

namespace LibraryApi.UnitTests;

public class BookServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly Mock<IAuthorRepository> _mockAuthorRepo;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBookRepo = new Mock<IBookRepository>();
        _mockAuthorRepo = new Mock<IAuthorRepository>();

        // Налаштовуємо зв'язок UnitOfWork з репозиторіями
        _mockUnitOfWork.Setup(u => u.Books).Returns(_mockBookRepo.Object);
        _mockUnitOfWork.Setup(u => u.Authors).Returns(_mockAuthorRepo.Object);

        _bookService = new BookService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllBooksAsync_WhenBooksExist_ReturnsListOfBookDtos()
    {
        // Arrange (Підготовка)
        var author = new Author { Id = 1, FirstName = "Тарас", LastName = "Шевченко" };
        var sampleBooks = new List<Book>
        {
            new Book { Id = 1, Title = "Кобзар", ISBN = "978-1", PublicationYear = 1840, Genre = "Поезія", AuthorId = 1, Author = author },
            new Book { Id = 2, Title = "Гайдамаки", ISBN = "978-2", PublicationYear = 1841, Genre = "Поема", AuthorId = 1, Author = author }
        };

        _mockBookRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sampleBooks);

        // Act (Дія)
        var result = await _bookService.GetAllBooksAsync();

        // Assert (Перевірка)
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Кобзар", result[0].Title);
        Assert.Equal("Тарас Шевченко", result[0].AuthorFullName);
        _mockBookRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdAsync_WhenBookExists_ReturnsBookDto()
    {
        // Arrange
        var author = new Author { Id = 2, FirstName = "Іван", LastName = "Франко" };
        var book = new Book
        {
            Id = 5,
            Title = "Захар Беркут",
            ISBN = "978-966-01-0002-2",
            PublicationYear = 1883,
            Genre = "Повість",
            AuthorId = 2,
            Author = author
        };

        _mockBookRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(book);

        // Act
        var result = await _bookService.GetBookByIdAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("Захар Беркут", result.Title);
        Assert.Equal("Іван Франко", result.AuthorFullName);
        _mockBookRepo.Verify(r => r.GetByIdAsync(5), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdAsync_WhenBookDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        _mockBookRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Book?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _bookService.GetBookByIdAsync(999));
        Assert.Contains("Book", ex.Message);
        Assert.Contains("999", ex.Message);
        _mockBookRepo.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateBookAsync_WhenAuthorExists_CreatesAndReturnsBookDto()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Лісова пісня",
            ISBN = "978-966-01-0003-3",
            PublicationYear = 1911,
            Genre = "Драма",
            AuthorId = 3
        };

        var author = new Author { Id = 3, FirstName = "Леся", LastName = "Українка" };

        _mockAuthorRepo.Setup(a => a.ExistsAsync(dto.AuthorId)).ReturnsAsync(true);
        _mockAuthorRepo.Setup(a => a.GetByIdAsync(dto.AuthorId)).ReturnsAsync(author);
        _mockBookRepo.Setup(b => b.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _bookService.CreateBookAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Лісова пісня", result.Title);
        Assert.Equal("Леся Українка", result.AuthorFullName);

        _mockAuthorRepo.Verify(a => a.ExistsAsync(dto.AuthorId), Times.Once);
        _mockBookRepo.Verify(b => b.AddAsync(It.IsAny<Book>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBookAsync_WhenAuthorDoesNotExist_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "Фантастична книга",
            ISBN = "978-0000000000",
            PublicationYear = 2026,
            Genre = "Фантастика",
            AuthorId = 999
        };

        _mockAuthorRepo.Setup(a => a.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _bookService.CreateBookAsync(dto));
        Assert.Contains("999", ex.Message);

        // Перевіряємо, що додавання книги та збереження не викликалися
        _mockBookRepo.Verify(b => b.AddAsync(It.IsAny<Book>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateBookAsync_WhenBookExistsAndAuthorExists_UpdatesBookAndSavesChanges()
    {
        // Arrange
        int bookId = 10;
        var existingBook = new Book
        {
            Id = bookId,
            Title = "Стара назва",
            ISBN = "111",
            PublicationYear = 2000,
            Genre = "Жанр",
            AuthorId = 1
        };

        var updateDto = new UpdateBookDto
        {
            Title = "Оновлена назва",
            ISBN = "222",
            PublicationYear = 2024,
            Genre = "Новий жанр",
            AuthorId = 2
        };

        _mockBookRepo.Setup(b => b.GetByIdAsync(bookId)).ReturnsAsync(existingBook);
        _mockAuthorRepo.Setup(a => a.ExistsAsync(updateDto.AuthorId)).ReturnsAsync(true);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _bookService.UpdateBookAsync(bookId, updateDto);

        // Assert
        Assert.Equal("Оновлена назва", existingBook.Title);
        Assert.Equal("222", existingBook.ISBN);
        Assert.Equal(2024, existingBook.PublicationYear);
        Assert.Equal(2, existingBook.AuthorId);

        _mockBookRepo.Verify(b => b.Update(existingBook), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_WhenBookExists_DeletesBookAndSavesChanges()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Книга на видалення" };
        _mockBookRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _bookService.DeleteBookAsync(1);

        // Assert
        _mockBookRepo.Verify(r => r.Delete(book), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_WhenBookDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        _mockBookRepo.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Book?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _bookService.DeleteBookAsync(404));

        _mockBookRepo.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
