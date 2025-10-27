using BookLending.Api.Controllers;
using BookLending.Api.Models;
using BookLending.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.Api.Tests;

public class BooksControllerTests
{
    [Fact]
    public async Task GetAll_returns_ok_with_list()
    {
        var repo = new InMemoryBookRepository();
        await repo.AddAsync(new Book { Title = "TDD", Author = "K. Beck" });

        var controller = new BooksController(repo);
        var result = await controller.GetAll() as OkObjectResult;

        result.Should().NotBeNull();
        var books = result!.Value as IEnumerable<Book>;
        books!.Should().ContainSingle(b => b.Title == "TDD");
    }
}
