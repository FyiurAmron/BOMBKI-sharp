#!/usr/bin/env bash
# build_windows.sh - Windows cross-compile from Linux.
# Usage:  bash build_windows.sh [godot-editor-binary]
# Output paths come from export_presets.cfg; the zip is named after the
# exported binary.
# Works around a Godot 4.8-dev7 mono bug: the editor spawns `dotnet publish`
# with a broken MSBUILD_EXE_PATH (SDK dir + "MSBuild.dll" without separator),
# which breaks SDK resolution (MSB4236) and RID inference (MSB4044).
# Fix: put tools/dotnet-sanitize first on PATH so the shim unsets the
# poisoned MSBuild* variables before delegating to the real dotnet.
# Pure ASCII.
set -euo pipefail

REPO="$(readlink -f "$(dirname "$0")/..")"
eval "$(bash "$REPO/tools/install_deps.sh" --print-versions)"
# shellcheck source=tools/godot-editor.sh
. "$REPO/tools/godot-editor.sh"
GODOT="$(godot_editor "${1:-$GODOT_BIN}" "$GODOT_TAG")"
OUT="$REPO/build/windows"
export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export PATH="$REPO/tools/dotnet-sanitize:$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

echo "==> dotnet: $(which dotnet)"
dotnet --list-sdks
echo "==> Exporting Windows x86_64..."
rm -rf "$OUT"
mkdir -p "$OUT"
"$GODOT" --headless --path "$REPO" --export-release "Windows x86_64"
EXE="$(find "$OUT" -maxdepth 1 -type f -name '*.exe' | head -1)"
[ -n "$EXE" ] || { echo "export produced no exe in $OUT"; exit 1; }
STEM="$(basename "$EXE" .exe | tr '[:upper:]' '[:lower:]')"
ZIP="$REPO/build/${STEM}-windows-x86_64.zip"
echo "==> Zipping..."
rm -f "$ZIP"
"$REPO/tools/zip-dir.sh" "$OUT" "$ZIP"
ls -la "$OUT" "$ZIP"
