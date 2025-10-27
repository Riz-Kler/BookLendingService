using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BookLending.Api.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var ddbUrl = Environment.GetEnvironmentVariable("DDB_SERVICE_URL") ?? "http://localhost:8000";
var ddbConfig = new AmazonDynamoDBConfig { ServiceURL = ddbUrl };

builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
    new AmazonDynamoDBClient(new BasicAWSCredentials("dummy", "dummy"), ddbConfig));

builder.Services.AddSingleton<IBookRepository, DynamoDbBookRepository>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
