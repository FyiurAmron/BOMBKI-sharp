#!/usr/bin/env bash
# tools/check-style.sh - verify repo style and fail listing offenders.
# Three checks: text hygiene (newline at EOF, no trailing whitespace, the
# same rules the pre-commit hook enforces), `dotnet format`
# --verify-no-changes over the solution, and the Roslyn-based
# tools/style-lint over src and tests.
# The lint's Bag.Count findings are known structural exceptions, not style
# debt: Bag.Count is a short item counter mirroring the Pascal original,
# not a collection, so !Any() cannot apply. They are the only findings
# the gate tolerates, matched on the flagged source line itself.
# Exits 1 and lists offenders when any other check fails. Pure ASCII.
set -euo pipefail

SCRIPT_DIR="$(dirname "$(readlink -f "$0")")"
ROOT="$(readlink -f "$SCRIPT_DIR/..")"
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

command -v dotnet >/dev/null || { echo "need dotnet on PATH"; exit 2; }

echo "==> CS: dotnet format --verify-no-changes..."
if ! dotnet format "$ROOT/BombkiSharp.slnx" --verify-no-changes --no-restore; then
    fail=1
fi

echo "==> CS lint: style-lint over src and tests..."
lint_out="$(cd "$ROOT" && dotnet run --project "$SCRIPT_DIR/style-lint" -- src tests || true)"
if [ -n "$lint_out" ]; then
    while IFS= read -r finding; do
        file="$(printf '%s' "$finding" | sed -E 's/^ *([^:]+): line ([0-9]+):.*/\1/')"
        line="$(printf '%s' "$finding" | sed -E 's/^ *([^:]+): line ([0-9]+):.*/\2/')"
        if sed -n "${line}p" "$ROOT/$file" | grep -q "Bag\.Count"; then
            continue
        fi
        echo "  $finding"
        fail=1
    done <<< "$lint_out"
fi

exit $fail
