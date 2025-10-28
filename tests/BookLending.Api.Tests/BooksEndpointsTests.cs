using BookLending.Api.Controllers;
using BookLending.Api.Models;
using BookLending.Api.Repositories;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;

namespace BookLending.Api.Tests;

public class BooksEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task GetBooks_returns_200_and_json()
    {
        var resp = await _client.GetAsync("/books");
        resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        (await resp.Content.ReadAsStringAsync()).Should().NotBeNull();
    }
}
