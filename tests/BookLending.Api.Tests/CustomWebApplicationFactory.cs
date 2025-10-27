using System.Linq;
using BookLending.Api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BookLending.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing IBookRepository registrations (Dynamo/InMemory)
            var toRemove = services.Where(s => s.ServiceType == typeof(IBookRepository)).ToList();
            foreach (var s in toRemove) services.Remove(s);

            // Add in-memory repo for deterministic tests
            services.AddSingleton<IBookRepository, InMemoryBookRepository>();
        });
    }
}
