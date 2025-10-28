using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BookLending.Api.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookLending.Api.Repositories;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    // Optional: include XML comments (enable in csproj as below)
    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath)) o.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BookLending API",
        Version = "v1",
        Description = "Simple book lending API with DynamoDB or in-memory repo."
    });
});

// Read from configuration (works for appsettings, env vars, in-memory, etc.)
var ddbUrl  = builder.Configuration["DDB_SERVICE_URL"];
var region  = builder.Configuration["AWS_REGION"] ?? "eu-west-2";

IAmazonDynamoDB ddbClient =
    ddbUrl is not null
        ? new AmazonDynamoDBClient(
              new BasicAWSCredentials("dummy","dummy"),
              new AmazonDynamoDBConfig { ServiceURL = ddbUrl })
        : new AmazonDynamoDBClient(new AmazonDynamoDBConfig {
              RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
          });

builder.Services.AddSingleton(ddbClient);

// Register your repository (switch to DynamoDbBookRepository when needed)
builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookLending API v1");
        c.RoutePrefix = "swagger"; // UI at /swagger
    });
}
app.MapControllers();
app.Run();

// For WebApplicationFactory discovery in tests:
public partial class Program { }
