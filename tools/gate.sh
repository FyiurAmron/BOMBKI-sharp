#!/usr/bin/env bash
# tools/gate.sh - the full local verification gate: style, then tests.
# Fails with the first failing step. Pure ASCII.
set -euo pipefail
ROOT="$(readlink -f "$(dirname "$0")/..")"
bash "$ROOT/tools/check-style.sh"
bash "$ROOT/tools/unit-test.sh"
