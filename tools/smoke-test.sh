#!/usr/bin/env bash
# tools/smoke-test.sh - run the headless frontend smoke test.
# Builds first: the headless editor does not rebuild the game assembly,
# so a smoke on a stale DLL tests old code. Fails red. Pure ASCII.
set -euo pipefail
ROOT="$(readlink -f "$(dirname "$0")/..")"
bash "$ROOT/tools/build.sh"
command -v godot >/dev/null || { echo "need godot on PATH" >&2; exit 2; }
godot --headless --path "$ROOT" -- --smoke
