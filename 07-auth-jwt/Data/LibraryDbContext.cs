using Microsoft.EntityFrameworkCore;
using LibraryApi.Models;

namespace LibraryApi.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "Тарас", LastName = "Шевченко", Country = "Україна" },
            new Author { Id = 2, FirstName = "Джордж", LastName = "Орвелл", Country = "Велика Британія" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Кобзар", ISBN = "978-966-03-7123-1", PublicationYear = 1840, Genre = "Поезія", AuthorId = 1 },
            new Book { Id = 2, Title = "1984", ISBN = "978-0-452-28423-4", PublicationYear = 1949, Genre = "Антиутопія", AuthorId = 2 }
        );
    }
}
