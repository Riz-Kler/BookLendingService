# 📚 BookLendingService  
[![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet)](https://dotnet.microsoft.com/)
[![Tests Passing](https://img.shields.io/badge/tests-passing-brightgreen?logo=xunit)](https://github.com/Riz-Kler/BookLendingService/releases/tag/v1.1.0-test-suite)
[![xUnit](https://img.shields.io/badge/tested%20with-xUnit-orange?logo=xunit)](https://xunit.net/)
[![License](https://img.shields.io/badge/license-MIT-lightgrey)](LICENSE)
[![GitHub release](https://img.shields.io/github/v/release/Riz-Kler/BookLendingService?label=latest%20release&color=success)](https://github.com/Riz-Kler/BookLendingService/releases/latest)

---

✅ **Latest Release:** [v1.1.0-test-suite](https://github.com/Riz-Kler/BookLendingService/releases/tag/v1.1.0-test-suite)  
🧪 **Includes:** Full integration test suite using FluentAssertions, WebApplicationFactory, and xUnit  
📦 **Framework:** .NET 8.0 (minimal API, in-memory hosting)  
🧰 **CI-Ready:** Can run locally or in future GitHub Actions workflows  

---

## 🧪 Running the Test Suite Locally

You can execute all tests **without** starting the API or Docker — everything runs using the in-memory test server.

### 🪟 **Windows (PowerShell)**

1. Allow PowerShell script execution (one-time only):
   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass




BookLendingService
Overview

ASP.NET Core 8 Web API for managing a simple book lending system.
Uses Amazon DynamoDB as the datastore and supports both:

Local development with DynamoDB Local

AWS deployment on ECS Fargate using Terraform (Infrastructure as Code)

1. Run Locally
Prerequisites

.NET 8 SDK

Docker Desktop

AWS CLI v2

Start DynamoDB Local
docker run -d --name dynamodb-local -p 8000:8000 amazon/dynamodb-local

Create the DynamoDB Table
aws dynamodb create-table \
  --table-name Books \
  --attribute-definitions AttributeName=Id,AttributeType=S \
  --key-schema AttributeName=Id,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST \
  --endpoint-url http://localhost:8000 \
  --region eu-west-2

Run the API
dotnet run --project "src/BookLending.Api/BookLending.Api.csproj" --urls "http://localhost:5280"

Test the Endpoints
# Add a book
curl -X POST http://localhost:5280/books \
  -H "Content-Type: application/json" \
  -d '{ "title":"Clean Code", "author":"Robert C. Martin" }'

# List all books
curl http://localhost:5280/books

# Checkout / Return a book
curl -X POST http://localhost:5280/books/{id}/checkout
curl -X POST http://localhost:5280/books/{id}/return

2. Optional: Run with Docker Compose

Create a docker-compose.yml in the project root:

version: "3.9"
services:
  dynamodb:
    image: amazon/dynamodb-local
    container_name: dynamodb-local
    ports:
      - "8000:8000"

  api:
    build: .
    container_name: book-lending-api
    environment:
      DDB_SERVICE_URL: http://dynamodb:8000
      ASPNETCORE_URLS: http://0.0.0.0:8080
    ports:
      - "8080:8080"
    depends_on:
      - dynamodb


Run both containers:

docker compose up --build -d


Then test:

curl http://localhost:8080/books

3. Containerization (Dockerfile)

A Dockerfile is already included. It uses multi-stage .NET publish and exposes port 8080.

docker build -t book-lending-api .
docker run -p 8080:8080 \
  -e DDB_SERVICE_URL=http://host.docker.internal:8000 \
  book-lending-api

4. Deploy to AWS (ECS Fargate via Terraform)
Step 1 – Build and Push to ECR
AWS_REGION=eu-west-2
APP_NAME=book-lending-api
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)

aws ecr create-repository --repository-name $APP_NAME --region $AWS_REGION 2>/dev/null || true

aws ecr get-login-password --region $AWS_REGION \
| docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com

docker build -t $APP_NAME .
docker tag $APP_NAME:latest $ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/$APP_NAME:latest
docker push $ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/$APP_NAME:latest

Step 2 – Configure AWS Credentials (PowerShell example)
aws configure --profile booklending
$env:AWS_PROFILE = "booklending"
aws sts get-caller-identity
$env:ACCOUNT_ID = (aws sts get-caller-identity --query Account --output text)

Step 3 – Apply Terraform

From your project root:

cd infra/terraform

terraform fmt
terraform init -upgrade
terraform validate

terraform apply -auto-approve \
  -var "region=eu-west-2" \
  -var "image_uri=$ACCOUNT_ID.dkr.ecr.eu-west-2.amazonaws.com/book-lending-api:latest"


When prompted for image_uri, enter:

123456789012.dkr.ecr.eu-west-2.amazonaws.com/book-lending-api:latest

Step 4 – Test the Deployed API

After deployment, Terraform outputs your ALB DNS name:

alb_dns_name = "booklending-api-alb-xxxxxxx.eu-west-2.elb.amazonaws.com"


Test the API:

curl http://booklending-api-alb-xxxxxxx.eu-west-2.elb.amazonaws.com/books

5. Notes

Program.cs automatically connects to the local DynamoDB endpoint (DDB_SERVICE_URL) when running locally.

In AWS, the ECS task uses IAM role-based authentication and connects to the DynamoDB regional endpoint.

Terraform provisions:

DynamoDB Table

ECS Cluster / Task Definition / Service

Application Load Balancer (ALB)

CloudWatch Logs

IAM roles & policies

Security Groups

6. Troubleshooting
Issue	Fix
“Could not connect to server”	Use port 5280 for local run or 8080 in Docker
“You must specify a region”	Add --region eu-west-2 or run aws configure
File locked during rebuild	Run taskkill /IM dotnet.exe /F
Invalid AWS token	Run aws configure --profile booklending again
Dockerfile not found	Ensure you run docker build from the project root
Author

README tweak

Ports:

dotnet run → http://localhost:5280

Docker/Compose → http://localhost:8080

Env var for local: set DDB_SERVICE_URL=http://localhost:8000 when running locally or pass via Docker env.

Create table locally: show the exact aws dynamodb create-table ... --endpoint-url http://localhost:8000 command (as above).

Common errors:

Locked exe (MSB3021/MSB3027): kill dotnet.exe and delete bin/obj.

InvalidClientTokenId: you forgot --endpoint-url and hit real AWS.


## Notes
This implementation includes a Swagger UI adapted from a prior .NET API project to accelerate development. 
All configurations have been refactored for the BookLendingService context and verified against DynamoDB Local.

## 🚀 Next steps

### 1) CI: run tests + coverage on every push/PR
Create `.github/workflows/tests.yml`:

```yaml
name: .NET Tests

on:
  push:
    branches: [ develop, main ]
  pull_request:
    branches: [ develop, main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --configuration Debug --no-restore
      - name: Test (with coverage)
        run: dotnet test tests/BookLending.Api.Tests/BookLending.Api.Tests.csproj --configuration Debug --no-build --collect:"XPlat Code Coverage" --results-directory TestResults
      - name: Upload coverage artifact
        uses: actions/upload-artifact@v4
        with:
          name: coverage
          path: TestResults


Rizwan Kler
October 2025