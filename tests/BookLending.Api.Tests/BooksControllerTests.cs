using BookLending.Api.Controllers;
using BookLending.Api.Models;
using BookLending.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Net.Http;                    
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing; 
using Xunit;


namespace BookLending.Api.Tests;

public class BooksControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksControllerTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task GetBooks_Returns200()
    {
        var resp = await _client.GetAsync("/books");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}