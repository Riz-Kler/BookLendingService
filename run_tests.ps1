Param(
  [switch]$Coverage
)

$ErrorActionPreference = "Stop"

Write-Host "`n=== Cleaning and restoring ==="
dotnet clean
dotnet restore

# Kill any orphaned API process that might lock the apphost
Write-Host "`n=== Ensuring API isn't running (to avoid file locks) ==="
try { taskkill /IM "BookLending.Api.exe" /F | Out-Null } catch {}

# Build Debug (UseAppHost=false in Debug prevents locky apphost issues)
Write-Host "`n=== Building (Debug) ==="
dotnet build -c Debug --no-restore

$results = "TestResults"
$newest = Join-Path $results (Get-Date -Format "yyyyMMdd_HHmmss")
New-Item -ItemType Directory -Path $newest -Force | Out-Null

Write-Host "`n=== Running tests ==="
$testProject = "tests/BookLending.Api.Tests/BookLending.Api.Tests.csproj"

if ($Coverage) {
  dotnet test $testProject -c Debug --no-build `
    --logger "trx;LogFileName=$newest\results.trx" `
    --collect:"XPlat Code Coverage" `
    --results-directory $newest `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
} else {
  dotnet test $testProject -c Debug --no-build `
    --logger "trx;LogFileName=$newest\results.trx" `
    --results-directory $newest
}

Write-Host "`n✅ Done. Results in $newest"
