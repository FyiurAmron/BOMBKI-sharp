#!/usr/bin/env bash
# tools/build.sh - build the game solution.
# Fails with the compiler errors. Pure ASCII.
set -euo pipefail
export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"
ROOT="$(readlink -f "$(dirname "$0")/..")"
dotnet build "$ROOT/bowcki.sln" --nologo -v q
