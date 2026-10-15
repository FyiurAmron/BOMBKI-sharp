#!/usr/bin/env bash
# build_linux.sh - native Linux export.
# Usage:  bash build_linux.sh [godot-editor-binary]
# Output paths come from export_presets.cfg; the zip is named after the
# exported binary.
# Uses the same dotnet sanitize shim as build_windows.sh: Godot 4.8-dev7
# mono spawns `dotnet publish` with a broken MSBUILD_EXE_PATH, which the
# shim unsets before delegating to the real dotnet.
# Pure ASCII.
set -euo pipefail

REPO="$(readlink -f "$(dirname "$0")/..")"
eval "$(bash "$REPO/tools/install_deps.sh" --print-versions)"
# shellcheck source=tools/godot-editor.sh
. "$REPO/tools/godot-editor.sh"
GODOT="$(godot_editor "${1:-$GODOT_BIN}" "$GODOT_TAG")"
OUT="$REPO/build/linux"
export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export PATH="$REPO/tools/dotnet-sanitize:$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

echo "==> dotnet: $(which dotnet)"
dotnet --list-sdks
echo "==> Exporting Linux x86_64..."
rm -rf "$OUT"
mkdir -p "$OUT"
"$GODOT" --headless --path "$REPO" --export-release "Linux x86_64"
BIN="$(find "$OUT" -maxdepth 1 -type f -name '*.x86_64' | head -1)"
[ -n "$BIN" ] || { echo "export produced no binary in $OUT"; exit 1; }
STEM="$(basename "$BIN" .x86_64 | tr '[:upper:]' '[:lower:]')"
ZIP="$REPO/build/${STEM}-linux-x86_64.zip"
echo "==> Zipping..."
rm -f "$ZIP"
"$REPO/tools/zip-dir.sh" "$OUT" "$ZIP"
ls -la "$OUT" "$ZIP"
