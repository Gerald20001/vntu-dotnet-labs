using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
    void Update(Book book);
    void Delete(Book book);
}

public interface IAuthorRepository
{
    Task<bool> ExistsAsync(int id);
    Task<Author?> GetByIdAsync(int id);
}

public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    Task<int> SaveChangesAsync();
}
