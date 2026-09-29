using Microsoft.EntityFrameworkCore;
using LibraryApi.Models;

namespace LibraryApi.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Налаштування зв'язку 1-до-багатьох: один Автор -> багато Книг
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Початкові (Seed) дані для демонстрації
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "Тарас", LastName = "Шевченко", Country = "Україна" },
            new Author { Id = 2, FirstName = "Джордж", LastName = "Орвелл", Country = "Велика Британія" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Кобзар", ISBN = "978-966-03-7123-1", PublicationYear = 1840, Genre = "Поезія", AuthorId = 1 },
            new Book { Id = 2, Title = "Гайдамаки", ISBN = "978-966-03-7124-8", PublicationYear = 1841, Genre = "Поема", AuthorId = 1 },
            new Book { Id = 3, Title = "1984", ISBN = "978-0-452-28423-4", PublicationYear = 1949, Genre = "Антиутопія", AuthorId = 2 },
            new Book { Id = 4, Title = "Колгосп тварин", ISBN = "978-0-452-28424-1", PublicationYear = 1945, Genre = "Сатира", AuthorId = 2 }
        );
    }
}
