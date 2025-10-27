using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using BookLending.Api.Models;

namespace BookLending.Api.Repositories;

public class DynamoDbBookRepository : IBookRepository
{
    private readonly DynamoDBContext _context;

    public DynamoDbBookRepository(IAmazonDynamoDB dynamoClient)
    {
        _context = new DynamoDBContext(dynamoClient);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        var results = await _context.ScanAsync<Book>(new List<ScanCondition>()).GetRemainingAsync();
        return results;
    }

    public async Task<Book?> GetByIdAsync(Guid id) =>
        await _context.LoadAsync<Book>(id);

    public async Task AddAsync(Book book) =>
        await _context.SaveAsync(book);

    public async Task UpdateAsync(Book book) =>
        await _context.SaveAsync(book);
}
