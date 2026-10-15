#!/usr/bin/env bash
# tools/check-style.sh - verify style with dedicated tools.
# CS goes through dotnet format (.editorconfig rules); data and config YAML
# through tools/yaml-lint (NanoYaml parse plus the data-file rules); JSON
# through jq, for the files that are still JSON; text hygiene through
# tools/check-text-hygiene.sh, the same code the pre-commit hook runs.
# Exits 1 and lists offenders when any check fails. Pure ASCII.
set -euo pipefail

SCRIPT_DIR="$(dirname "$(readlink -f "$0")")"
ROOT="$(readlink -f "$SCRIPT_DIR/..")"
fail=0

# shellcheck source=tools/check-text-hygiene.sh
. "$SCRIPT_DIR/check-text-hygiene.sh"

command -v dotnet >/dev/null || { echo "need dotnet on PATH"; exit 2; }
command -v jq >/dev/null || { echo "need jq on PATH"; exit 2; }

# Every project, found instead of named: no project name in the tooling.
echo "==> CS: dotnet format --verify-no-changes..."
while IFS= read -r proj; do
    if ! dotnet format "$proj" --verify-no-changes; then
        fail=1
    fi
done < <(find "$ROOT" -name '*.csproj' -not -path '*/.godot/*' | sort)

# The lint is a tiny project next to this script, kept out of
# the game assembly by bowcki.csproj; it parses through the
# engine's NanoYaml copy.
echo "==> YAML: NanoYaml parse, no comments or notes, even indent..."
if ! (cd "$ROOT" && dotnet run --project "$SCRIPT_DIR/yaml-lint" -- data config); then
    fail=1
fi

# The style lint is a tiny project next to this script, kept out of
# the game assembly by bowcki.csproj; it parses every game source
# with Roslyn and fails on a style violation.
echo "==> CS lint: collections, loops, error periods, test vars..."
if ! (cd "$ROOT" && dotnet run --project "$SCRIPT_DIR/style-lint" -- engine frontend test _tools); then
    fail=1
fi

# No JSON data left today, but the check stays: the next JSON file should not
# have to reinvent the gate.
echo "==> JSON: jq canonical pretty-print..."
while IFS= read -r f; do
    if ! jq -a . "$f" | cmp -s - "$f"; then
        echo "    non-canonical JSON: $f"
        fail=1
    fi
done < <(find "$ROOT/data" "$ROOT/config" -name '*.json' | sort)

echo "==> Text: newline at EOF, no trailing whitespace, docs within 80/120..."
if ! (cd "$ROOT" && check_text_files $(git ls-files)); then
    fail=1
fi

exit $fail
