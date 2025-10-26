using BookLending.Api.Models;

namespace BookLending.Api.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(Guid id);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
}
