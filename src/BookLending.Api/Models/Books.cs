using Amazon.DynamoDBv2.DataModel;

namespace BookLending.Api.Models;

[DynamoDBTable("Books")]
public class Book
{
    [DynamoDBHashKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}
