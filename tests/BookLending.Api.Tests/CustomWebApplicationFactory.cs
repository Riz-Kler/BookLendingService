using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookLending.Api.Tests; 

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["DDB_SERVICE_URL"] = "http://localhost:8000",
                ["AWS_REGION"]      = "eu-west-2"
            };
            config.AddInMemoryCollection(dict);
        });

        // only if you need to replace services for tests
        builder.ConfigureTestServices(services =>
        {
            // e.g., swap real AWS clients with fakes
        });
    }
}
