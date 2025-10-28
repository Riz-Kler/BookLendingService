using System.Net;
using System.Net.Http.Json;
using BookLending.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;

namespace BookLending.Api.Tests;

public class BooksEndpointsFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksEndpointsFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    // Helper to create a fresh book and return its Guid
    private async Task<Guid> CreateBookAsync(string title = "Clean Architecture", string author = "Robert C. Martin")
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            IsAvailable = true
        };

        var resp = await _client.PostAsJsonAsync("/books", book);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        resp.Headers.Location.Should().NotBeNull();
        return book.Id;
    }

    [Fact]
    public async Task PostBooks_creates_book_and_returns_201_with_Location()
    {
        var id = await CreateBookAsync("Domain-Driven Design", "Eric Evans");

        var list = await _client.GetFromJsonAsync<List<Book>>("/books");
        list!.Any(b => b.Id == id && b.Title == "Domain-Driven Design").Should().BeTrue();
    }

    [Fact]
    public async Task PostBooks_invalid_payload_returns_400()
    {
        var bad = new
        {
            id = "not-a-guid",
            author = "Nobody",
            isAvailable = true
        };

        var resp = await _client.PostAsJsonAsync("/books", bad);
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await resp.Content.ReadAsStringAsync();
        body.Should().MatchRegex("(could not be converted|Invalid|required|The value)");
    }

    [Fact]
    public async Task Checkout_then_Return_updates_availability_flags()
    {
        var id = await CreateBookAsync("Refactoring", "Martin Fowler");

        var checkoutResp = await _client.PostAsync($"/books/{id}/checkout", content: null);
        checkoutResp.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        var afterCheckout = await _client.GetFromJsonAsync<List<Book>>("/books");
        afterCheckout!.Single(b => b.Id == id).IsAvailable.Should().BeFalse();

        var returnResp = await _client.PostAsync($"/books/{id}/return", content: null);
        returnResp.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        var afterReturn = await _client.GetFromJsonAsync<List<Book>>("/books");
        afterReturn!.Single(b => b.Id == id).IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task Checkout_twice_returns_conflict_or_bad_request()
    {
        var id = await CreateBookAsync("The Pragmatic Programmer", "Hunt & Thomas");

        // First checkout should succeed: 200 or 204
        (await _client.PostAsync($"/books/{id}/checkout", null))
            .StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // Second checkout: depending on implementation could be 409/400 or idempotent 204
        var second = await _client.PostAsync($"/books/{id}/checkout", null);
        second.StatusCode.Should().BeOneOf(HttpStatusCode.Conflict, HttpStatusCode.BadRequest, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Return_when_not_checked_out_returns_conflict_or_bad_request()
    {
        var id = await CreateBookAsync("Working Effectively with Legacy Code", "Michael Feathers");
        var resp = await _client.PostAsync($"/books/{id}/return", null);
        resp.StatusCode.Should().BeOneOf(HttpStatusCode.Conflict, HttpStatusCode.BadRequest, HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
    [InlineData("not-a-guid")]
    public async Task Checkout_with_invalid_id_returns_400_or_404(string rawId)
    {
        var resp = await _client.PostAsync($"/books/{rawId}/checkout", null);
        resp.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }
}
