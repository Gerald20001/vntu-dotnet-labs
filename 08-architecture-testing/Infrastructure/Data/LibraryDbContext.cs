using Microsoft.EntityFrameworkCore;
using LibraryApi.Domain.Entities;

namespace LibraryApi.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<BookLoan> BookLoans => Set<BookLoan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Author configuration
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.LastName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Country).HasMaxLength(100);
        });

        // Book configuration
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            entity.Property(b => b.Genre).IsRequired().HasMaxLength(100);

            entity.HasOne(b => b.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Reader configuration
        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.FullName).IsRequired().HasMaxLength(150);
            entity.Property(r => r.Email).IsRequired().HasMaxLength(100);
            entity.Property(r => r.TicketNumber).IsRequired().HasMaxLength(20);
        });

        // BookLoan configuration
        modelBuilder.Entity<BookLoan>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.HasOne(l => l.Book)
                  .WithMany(b => b.BookLoans)
                  .HasForeignKey(l => l.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Reader)
                  .WithMany(r => r.BookLoans)
                  .HasForeignKey(l => l.ReaderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Початкові дані (Seed data)
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "Тарас", LastName = "Шевченко", Country = "Україна" },
            new Author { Id = 2, FirstName = "Іван", LastName = "Франко", Country = "Україна" },
            new Author { Id = 3, FirstName = "Леся", LastName = "Українка", Country = "Україна" },
            new Author { Id = 4, FirstName = "Джордж", LastName = "Орвелл", Country = "Велика Британія" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Кобзар", ISBN = "978-966-01-0001-1", PublicationYear = 1840, Genre = "Поезія", AuthorId = 1 },
            new Book { Id = 2, Title = "Захар Беркут", ISBN = "978-966-01-0002-2", PublicationYear = 1883, Genre = "Історична повість", AuthorId = 2 },
            new Book { Id = 3, Title = "Лісова пісня", ISBN = "978-966-01-0003-3", PublicationYear = 1911, Genre = "Драма-феєрія", AuthorId = 3 },
            new Book { Id = 4, Title = "1984", ISBN = "978-0-452-28423-4", PublicationYear = 1949, Genre = "Антиутопія", AuthorId = 4 }
        );
    }
}
