#!/usr/bin/env bash
set -euo pipefail

echo "🔧 Stopping any stale BookLending.Api processes..."
# Best-effort kills; ignore failures if not running
pkill -f "BookLending.Api" 2>/dev/null || true
pkill -f "src/BookLending.Api/bin/Debug/net8.0/BookLending.Api" 2>/dev/null || true

echo "🧹 dotnet clean"
dotnet clean

echo "📦 dotnet restore"
dotnet restore

ARGS=()
if [[ "${1:-}" == "--coverage" ]]; then
  ARGS+=(--collect:"XPlat Code Coverage")
fi

echo "🧪 Running tests..."
dotnet test -c Debug "${ARGS[@]}"

echo "✅ Tests completed."
