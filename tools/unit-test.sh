#!/usr/bin/env bash
# tools/unit-test.sh - run the data contract tests.
# Fails with the test failures. Pure ASCII.
set -euo pipefail
export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"
ROOT="$(readlink -f "$(dirname "$0")/..")"
dotnet test "$ROOT/test/DataTests/DataTests.csproj" --nologo 2>&1 | tail -n 2
