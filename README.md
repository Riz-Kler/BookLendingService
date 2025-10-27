BookLendingService – DynamoDB Local Implementation
1. Overview

This project implements a simple Book Lending API using .NET 8 and AWS DynamoDB Local.
It allows you to add, list, check out, and return books.
All data is stored in a local DynamoDB instance running in Docker.

2. Prerequisites

.NET 8 SDK

Docker Desktop (required for DynamoDB Local)

AWS CLI (optional, used to create the table)

3. Running DynamoDB Local in Docker

Make sure Docker Desktop is running.

Open PowerShell and run from the BookLendingService folder:

docker run -d --name dynamodb-local -p 8000:8000 amazon/dynamodb-local


Verify that the container is running:

docker ps


You should see amazon/dynamodb-local listed.

If you rebuild often, you can remove and restart the container:

docker stop dynamodb-local && docker rm dynamodb-local
docker run -d --name dynamodb-local -p 8000:8000 amazon/dynamodb-local

4. Create the DynamoDB Table

Run this command to create the Books table (from any terminal with AWS CLI configured):

For Command Prompt:

aws dynamodb create-table ^
  --table-name Books ^
  --attribute-definitions AttributeName=Id,AttributeType=S ^
  --key-schema AttributeName=Id,KeyType=HASH ^
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 ^
  --endpoint-url http://localhost:8000


For PowerShell:
Use backticks (`) instead of carets, or paste the command as a single line.

5. Run the API

From the BookLendingService folder:

dotnet run --project "src/BookLending.Api/BookLending.Api.csproj" --urls "http://localhost:5280"


The API will start and connect automatically to DynamoDB Local at
http://localhost:8000.

6. Test the API

Open a new terminal and run the following commands.

Add a book

curl -X POST http://localhost:5280/books ^
  -H "Content-Type: application/json" ^
  -d "{ \"title\": \"Clean Code\", \"author\": \"Robert C. Martin\" }"


List all books

curl http://localhost:5280/books


Check out a book

curl -X POST http://localhost:5280/books/{id}/checkout


Return a book

curl -X POST http://localhost:5280/books/{id}/return


Replace {id} with the Id returned from the add command.
Each check-out or return updates the isAvailable flag in DynamoDB.

7. Optional – Run the API in Docker

If you want to containerise the API itself, make sure you have the Dockerfile in the project root:

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish "src/BookLending.Api/BookLending.Api.csproj" -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "BookLending.Api.dll"]


Then build and run:

docker build -t book-lending-api .
docker run -p 8080:8080 book-lending-api


If DynamoDB Local runs outside the API container, set the ServiceURL in Program.cs to:

new AmazonDynamoDBConfig { ServiceURL = "http://host.docker.internal:8000" }

8. Troubleshooting

File locked during build

error MSB3021: Unable to copy BookLending.Api.exe because it is being used by another process


Run:

taskkill /IM dotnet.exe /F


Then rebuild.

DynamoDBContext warning

'DynamoDBContext.DynamoDBContext(IAmazonDynamoDB)' is obsolete


This is safe to ignore. It only warns that AWS now recommends DynamoDBContextBuilder.

Verify container status

docker ps


Ensure that amazon/dynamodb-local is running on port 8000.