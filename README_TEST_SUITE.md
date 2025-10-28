# Test Suite — BookLendingService

This guide shows how to run the API tests **without** starting the app or Docker.

## Prerequisites
- .NET SDK 8 (or newer 8.x)
- PowerShell (Windows) or bash (macOS/Linux)

## Structure
- `tests/BookLending.Api.Tests` — xUnit tests (FluentAssertions, WebApplicationFactory)
- Runners:
  - `run_tests.ps1` (Windows / PowerShell)
  - `run_tests.sh` (macOS/Linux)

> The test host spins up the API **in-process**; do **not** run `dotnet run` or Docker.

## Quick Start

### Windows (PowerShell)
```powershell
# one-off: allow this session to run the script
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# run tests
.\run_tests.ps1

# with coverage
.\run_tests.ps1 -Coverage


### macOS / Linux (bash)
chmod +x ./run_tests.sh

# run tests
./run_tests.sh

# with coverage
./run_tests.sh --coverage