using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BookLending.Api.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// If DDB_SERVICE_URL is set (local/compose), use that.
// Otherwise, let the SDK use AWS regional endpoints + default credentials (task role).
var ddbUrl = Environment.GetEnvironmentVariable("DDB_SERVICE_URL");

IAmazonDynamoDB ddbClient = ddbUrl is not null
    ? new AmazonDynamoDBClient(new BasicAWSCredentials("dummy", "dummy"),
        new AmazonDynamoDBConfig { ServiceURL = ddbUrl })
    : new AmazonDynamoDBClient(); // uses task role / default chain in AWS

builder.Services.AddSingleton(ddbClient);
builder.Services.AddSingleton<IBookRepository, DynamoDbBookRepository>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
