BookLendingService — Quick Start
Simple book-lending API with .NET 8, Swagger, and DynamoDB Local.
Supports:


GET /books


POST /books (create)


POST /books/{id}/checkout


POST /books/{id}/return


Swagger Tests at the end of this README.


Model (see src/BookLending.Api/Models/Book.cs)

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}


1) Run locally (no Docker)
Create the table in DynamoDB Local
Open PowerShell in the repo root and run:
# Give AWS CLI dummy creds (DynamoDB Local accepts anything)
$env:AWS_ACCESS_KEY_ID    = "local"
$env:AWS_SECRET_ACCESS_KEY = "local"
$env:AWS_DEFAULT_REGION    = "eu-west-2"

# Create table 'Books'
aws dynamodb create-table `
  --table-name Books `
  --attribute-definitions AttributeName=Id,AttributeType=S `
  --key-schema AttributeName=Id,KeyType=HASH `
  --billing-mode PAY_PER_REQUEST `
  --endpoint-url http://localhost:8000

Start the API
# Let the API know where DynamoDB Local is
$env:DDB_SERVICE_URL = "http://localhost:8000"

dotnet run --project .\src\BookLending.Api\BookLending.Api.csproj --urls "http://localhost:5280"

Open http://localhost:5280/swagger/index.html.

2) Run with Docker Compose
From the repo root (where docker-compose.yml is):
docker compose up --build



API: http://localhost:5280


DynamoDB Local: bound to port 8000 inside the docker network



If port 8000 is “already allocated”, stop any stray container:

docker ps --format "{{.ID}} {{.Image}} {{.Ports}} {{.Names}}" | findstr /I "8000 dynamodb"
docker stop <container-name-or-id>


3) Seeding books (fixed GUIDs + authors)
You can seed via Swagger (“POST /books”) or with curl.
Use these three JSON payloads (copy one at a time into Swagger)
{
  "id": "00000000-0000-0000-0000-000000000001",
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isAvailable": true
}

{
  "id": "00000000-0000-0000-0000-000000000002",
  "title": "Test Driven Development: By Example",
  "author": "Kent Beck",
  "isAvailable": true
}

{
  "id": "00000000-0000-0000-0000-000000000003",
  "title": "Refactoring: Improving the Design of Existing Code",
  "author": "Martin Fowler",
  "isAvailable": true
}

Or seed with curl
curl -X POST http://localhost:5280/books -H "Content-Type: application/json" -d '{ "id":"00000000-0000-0000-0000-000000000001","title":"Clean Code","author":"Robert C. Martin","isAvailable":true }'
curl -X POST http://localhost:5280/books -H "Content-Type: application/json" -d '{ "id":"00000000-0000-0000-0000-000000000002","title":"Test Driven Development: By Example","author":"Kent Beck","isAvailable":true }'
curl -X POST http://localhost:5280/books -H "Content-Type: application/json" -d '{ "id":"00000000-0000-0000-0000-000000000003","title":"Refactoring: Improving the Design of Existing Code","author":"Martin Fowler","isAvailable":true }'

Verify & copy GUIDs


GET /books in Swagger to list all books.


Copy the id you want to loan.



4) Lending & returning (Swagger)


Checkout: POST /books/{id}/checkout


Return: POST /books/{id}/return


Paste the GUID you copied from GET /books.
If a book is already loaned out, checkout should return a 400 with a friendly error; return should succeed and set isAvailable back to true.

5) Running the test suite
dotnet clean
dotnet restore
dotnet build BookLendingService.sln -c Debug
dotnet test tests/BookLending.Api.Tests/BookLending.Api.Tests.csproj -c Debug


Note: tests spin up the API in-memory using WebApplicationFactory<Program> and configure the app with DDB_SERVICE_URL=http://localhost:8000. Make sure DynamoDB Local is running.


6) Tips & common pitfalls


400 Bad Request / GUID
The API expects a GUID for id. If you don’t supply one, the server will still generate it (Guid.NewGuid() in the model), but for the demo we seed known IDs so you can copy/paste them for checkout/return.


“No RegionEndpoint or ServiceURL configured”
Ensure DDB_SERVICE_URL is set (or provided by Docker Compose).


Port conflicts (8000)
Stop any old dynamodb-local container: docker stop dynamodb-local (or remove with docker rm -f dynamodb-local).



7) Optional: allow server to create the Id automatically
If you prefer to POST without an id, ensure your create endpoint does:
// inside POST /books handler
if (book.Id == Guid.Empty) book.Id = Guid.NewGuid();

You can still list with GET /books and copy the generated id for checkout/return.

8) Health check
curl http://localhost:5280/books
# should return [] (empty) or the books you seeded


That’s it!
You can now:


Start the API (local or Docker).


Seed the three books (known GUIDs).


GET /books → copy an id.


POST /books/{id}/checkout → then POST /books/{id}/return.



The three seed payloads (quick copy)



{ "id":"00000000-0000-0000-0000-000000000001","title":"Clean Code","author":"Robert C. Martin","isAvailable":true }




{ "id":"00000000-0000-0000-0000-000000000002","title":"Test Driven Development: By Example","author":"Kent Beck","isAvailable":true }




{ "id":"00000000-0000-0000-0000-000000000003","title":"Refactoring: Improving the Design of Existing Code","author":"Martin Fowler","isAvailable":true }

