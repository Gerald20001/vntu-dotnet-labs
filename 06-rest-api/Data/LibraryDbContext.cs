using System;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Models;

namespace LibraryApi.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<BookLoan> BookLoans => Set<BookLoan>();

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1-to-many: Author -> Books
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships for BookLoan
        modelBuilder.Entity<BookLoan>()
            .HasOne(bl => bl.Book)
            .WithMany(b => b.BookLoans)
            .HasForeignKey(bl => bl.BookId);

        modelBuilder.Entity<BookLoan>()
            .HasOne(bl => bl.Reader)
            .WithMany(r => r.BookLoans)
            .HasForeignKey(bl => bl.ReaderId);

        // Seed data
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "Тарас", LastName = "Шевченко", Country = "Україна" },
            new Author { Id = 2, FirstName = "Джордж", LastName = "Орвелл", Country = "Велика Британія" },
            new Author { Id = 3, FirstName = "Іван", LastName = "Франко", Country = "Україна" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Кобзар", ISBN = "978-966-03-7123-1", PublicationYear = 1840, Genre = "Поезія", AuthorId = 1 },
            new Book { Id = 2, Title = "Гайдамаки", ISBN = "978-966-03-7124-8", PublicationYear = 1841, Genre = "Поема", AuthorId = 1 },
            new Book { Id = 3, Title = "1984", ISBN = "978-0-452-28423-4", PublicationYear = 1949, Genre = "Антиутопія", AuthorId = 2 },
            new Book { Id = 4, Title = "Колгосп тварин", ISBN = "978-0-452-28424-1", PublicationYear = 1945, Genre = "Сатира", AuthorId = 2 },
            new Book { Id = 5, Title = "Захар Беркут", ISBN = "978-966-03-7125-5", PublicationYear = 1883, Genre = "Повість", AuthorId = 3 }
        );

        modelBuilder.Entity<Reader>().HasData(
            new Reader { Id = 1, FullName = "Мельник Андрій", Email = "andriy.melnyk@vntu.edu.ua", TicketNumber = "ТК-1001", PhoneNumber = "+380671112233" },
            new Reader { Id = 2, FullName = "Коваленко Олена", Email = "olena.kovalenko@vntu.edu.ua", TicketNumber = "ТК-1002", PhoneNumber = "+380672223344" }
        );

        modelBuilder.Entity<BookLoan>().HasData(
            new BookLoan { Id = 1, BookId = 1, ReaderId = 1, LoanDate = new DateTime(2026, 9, 15), DueDate = new DateTime(2026, 10, 15), IsReturned = false },
            new BookLoan { Id = 2, BookId = 3, ReaderId = 2, LoanDate = new DateTime(2026, 9, 1), DueDate = new DateTime(2026, 9, 20), ReturnDate = new DateTime(2026, 9, 18), IsReturned = true }
        );
    }
}
