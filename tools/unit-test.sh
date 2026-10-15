#!/usr/bin/env bash
# tools/unit-test.sh - run the full test suite (PTY scenarios plus unit tests).
# Fails with the test failures. Pure ASCII.
set -euo pipefail
ROOT="$(readlink -f "$(dirname "$0")/..")"
dotnet test "$ROOT/BombkiSharp.slnx" --nologo 2>&1 | tail -n 2
