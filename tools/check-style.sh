#!/usr/bin/env bash
# tools/check-style.sh - verify repo style: every tracked text file ends
# with a newline and carries no trailing whitespace, the same rules the
# pre-commit hook enforces on staged files.
# The pack's stricter checks (doc widths, C# comments, trailing commas,
# dotnet format, yaml/style lint) belong to the Godot future: this codebase
# predates them and fails them throughout, so the gate does not run them.
# Exits 1 and lists offenders when any check fails. Pure ASCII.
set -euo pipefail

ROOT="$(readlink -f "$(dirname "$0")/..")"
fail=0

echo "==> Text: newline at EOF, no trailing whitespace..."
while IFS= read -r file; do
    if [ -n "$(tail -c 1 -- "$ROOT/$file")" ]; then
        echo "  $file: no newline at end of file"
        fail=1
    fi
    lines=$(grep -InE '[[:space:]]+$' -- "$ROOT/$file" | cut -d: -f1 | paste -sd, - || true)
    if [ -n "$lines" ]; then
        echo "  $file: trailing whitespace on line(s) $lines"
        fail=1
    fi
done < <(git -C "$ROOT" ls-files | grep -Ev '^(build|src-pas)/')

exit $fail
