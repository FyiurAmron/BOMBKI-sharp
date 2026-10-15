#!/usr/bin/env bash
# tools/build.sh - build the game solution.
# Fails with the compiler errors. Pure ASCII.
set -euo pipefail
ROOT="$(readlink -f "$(dirname "$0")/..")"
dotnet build "$ROOT/BombkiSharp.slnx" --nologo -v q
