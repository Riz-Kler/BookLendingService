using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookLending.Api.Tests;

public class BooksEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost") // ignored; in-memory host
        });
    }

    [Fact]
    public async Task Swagger_UI_should_be_available_in_Development()
    {
        var resp = await _client.GetAsync("/swagger");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_books_initially_returns_empty_array()
    {
        var resp = await _client.GetAsync("/books");
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync();
        json.Trim().Should().Be("[]");
    }

    [Fact]
    public async Task Add_checkout_return_book_happy_path()
    {
        var create = new { title = "Clean Code", author = "Robert C. Martin" };

        var addResp = await _client.PostAsJsonAsync("/books", create);
        addResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var added = await addResp.Content.ReadFromJsonAsync<dynamic>();
        string id = added.id.ToString();

        var listResp = await _client.GetAsync("/books");
        listResp.EnsureSuccessStatusCode();
        var list = await listResp.Content.ReadFromJsonAsync<List<dynamic>>();
        list.Should().ContainSingle(b => (string)b.id == id);

        var checkout = await _client.PostAsync($"/books/{id}/checkout", null);
        checkout.EnsureSuccessStatusCode();

        var returned = await _client.PostAsync($"/books/{id}/return", null);
        returned.EnsureSuccessStatusCode();
    }
}
