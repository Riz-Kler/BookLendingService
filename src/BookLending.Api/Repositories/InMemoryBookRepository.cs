using BookLending.Api.Models;

namespace BookLending.Api.Repositories;

public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books = new();

    public Task<IEnumerable<Book>> GetAllAsync() =>
        Task.FromResult(_books.AsEnumerable());

    public Task<Book?> GetByIdAsync(Guid id) =>
        Task.FromResult(_books.FirstOrDefault(b => b.Id == id));

    public Task AddAsync(Book book)
    {
        _books.Add(book);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Book book)
    {
        var existing = _books.FirstOrDefault(x => x.Id == book.Id);
        if (existing is not null)
        {
            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.IsAvailable = book.IsAvailable;
        }
        return Task.CompletedTask;
    }
}
