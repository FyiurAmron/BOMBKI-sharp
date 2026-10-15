---
name: godot
description: Working with Godot game endgine. Use when touching scripts, exports or toolchain.
---

# Godot toolchain and exports

## Layout

- Split the program into a Godot-free engine and a Godot frontend: the engine is
  a class library with no `using Godot`, no Godot types, no file access, no
  timers - its project file carries no Godot reference, so the compiler enforces
  the split. The frontend owns nodes, rendering, file access, timers.
- `.uid` files beside the scripts are tracked in git (Godot script UIDs).

## Editor and toolchain traps

- The editor loads its .NET assemblies from a `GodotSharp` directory beside the
  executable. Installing the binary alone aborts the engine with `.NET:
  Assemblies not found`; install the editor and its assemblies together.
- `godot --version` prints dots where the tag has dashes: `4.8-dev7` comes back
  as `4.8.dev7.mono.official.<sha>`. Normalize with `tr '-' '.'` and require the
  `mono` marker, or a stable 4.x found on PATH passes as the pinned build.
- `PATH` reaches login and interactive shells only (`~/.profile` adds
  `$HOME/.local/bin`; `~/.bashrc` is interactive-only). Resolve the editor
  through a variable that pins its path rather than trusting PATH; a CI or agent
  shell then needs no dotfile edits at all.
- Never drop a `godot._sc_` file beside the editor: it switches Godot to
  self-contained mode, and user data (saves, settings) moves next to the
  executable instead of `~/.local/share/godot`.
- Export templates are a separate archive and a separate install step; a
  toolchain image does it in its own layer.
- Building the toolchain image takes minutes (SDK + editor + templates): run it
  in the background rather than under a short timeout.

## Running and verifying

- Toolchain environment:

  ```bash
  export DOTNET_ROOT="$HOME/.dotnet"
  export PATH="$HOME/.local/bin:$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
  ```

- A headless run (`godot --headless --path .`) executes the last compiled
  assembly, so build first; it stays green on stale DLLs, so wipe
  `.godot/mono/temp/bin` and `.godot/mono/temp/obj` and rebuild before trusting
  green.
- Trap: a test that binds through a nested public class as a type argument does
  not run the outer class's static initializer - that is how tests read config
  through a Godot-touching class without Godot. Keep Godot out of static
  initializers the tests can reach.
- MSBuild incremental builds sometimes no-op after edits; if a rebuild looks
  suspiciously fast (<1s), touch the sources and rebuild - the DLL hash must
  change.

## Fonts, theme and glyphs

- Theme overrides carry the look: `AddThemeFontOverride`,
  `AddThemeFontSizeOverride`, `AddThemeColorOverride`. Font faces and sizes come
  from config, not code.
- Runtime Unicode (button icons, the text cursor) ships in a data file as
  `\uXXXX` escapes - still ASCII bytes - loaded through a glyphs reader, and
  every glyph has an ASCII fallback (a display map, such as CP437, is the usual
  mechanism).

## Exports and packaging

- `export_filter="all_resources"` skips TextFile-typed files (`.txt` and
  friends). Everything the game reads at runtime must still be packed.
- `include_filter` (comma-separated globs) adds files in every export mode.
  Patterns match case-insensitively against the `res://` path and the bare path,
  and `*` crosses directories, so `data/*` covers all of `data/` including
  subdirectories. `*.import` files are always stripped.
- `all_resources` only picks up file types Godot knows as resources, so runtime
  files that need explicit inclusion are listed: YAML data/config and the
  bundled font assets. Rename a data file extension and check the pck, or the
  file silently stops shipping (a missing optional asset costs flair; a missing
  data file ends the game).
- A `.import` file with `importer="keep"` ships the source file byte-identical
  at its own path and is never reimported; `importer="skip"` omits the file from
  exports entirely. Deleting a `.import` does not stick: the next import or
  export reimports it with defaults. Commit every `.import` the game needs - one
  keeps a bundled image's pixels raw.
- Windows cross-compiles run through a wrapper that puts a sanitizer shim first
  on PATH. The shim unsets the broken `MSBuild*` variables Godot dev builds pass
  to `dotnet publish`.
- A .NET export copies referenced assemblies into `build/`, so a managed
  dependency needs no `include_filter` entry; check the export still runs (a
  headless invocation of the exported binary) before assuming it did.
- Every NuGet package used at runtime is listed in a notices file with its
  licence and upstream; add the entry in the same commit as the
  `PackageReference`.
- All resource paths live in one config file, read once at bootstrap into static
  fields; the one path the binary knows is the bootstrap config file itself.
