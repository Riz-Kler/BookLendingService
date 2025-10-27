using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using BookLending.Api.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// AWS DynamoDB Local setup
builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
    new AmazonDynamoDBClient(
        new BasicAWSCredentials("dummy", "dummy"),
        new AmazonDynamoDBConfig { ServiceURL = "http://localhost:8000" }));

builder.Services.AddSingleton<IBookRepository, DynamoDbBookRepository>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
