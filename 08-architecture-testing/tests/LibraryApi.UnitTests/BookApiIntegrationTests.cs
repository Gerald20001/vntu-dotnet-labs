using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using LibraryApi.Application.DTOs;

namespace LibraryApi.UnitTests;

public class BookApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BookApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllBooks_ReturnsSuccessStatusCodeAndJsonArray()
    {
        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var books = await response.Content.ReadFromJsonAsync<BookDto[]>();
        Assert.NotNull(books);
        Assert.NotEmpty(books);
    }

    [Fact]
    public async Task GetBookById_WhenBookExists_ReturnsOkAndBookDetails()
    {
        // Act
        var response = await _client.GetAsync("/api/books/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal(1, book.Id);
        Assert.Equal("Кобзар", book.Title);
    }

    [Fact]
    public async Task GetBookById_WhenBookDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/books/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newBook = new CreateBookDto
        {
            Title = "Інтеграційний тест книги",
            ISBN = "978-966-99-9999-9",
            PublicationYear = 2026,
            Genre = "Тестування",
            AuthorId = 1 // Тарас Шевченко (seeded)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", newBook);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);
        Assert.Equal("Інтеграційний тест книги", created.Title);
    }

    [Fact]
    public async Task CreateBook_WithNonExistentAuthor_ReturnsBadRequest()
    {
        // Arrange
        var newBook = new CreateBookDto
        {
            Title = "Книга з неіснуючим автором",
            ISBN = "978-966-00-0000-0",
            PublicationYear = 2026,
            Genre = "Помилка",
            AuthorId = 99999
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", newBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
