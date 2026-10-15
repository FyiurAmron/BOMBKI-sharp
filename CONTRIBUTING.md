# Contribution rules

Test-backed rewrite of the reconstructed original 1999' BOMBKI text
RPG game, using test-backed Pascal refactoring of the original
code to create a complete minimal C# implementation - feature-identical
(including bugs).

## Ground truth and work areas

* `src-pas/` - the frozen refactoring snapshot;
  read-only, never staged (enforced by `.githooks/pre-commit`).
* `src/` - the C# implementation sources; all porting work happens here.

## Functional dir layout
* `tests/` - all things related to running tests
* `tests/scenarios` - PTY test scenarios (also driven from xUnit)
* `tests/Bombki.Tests` - xUnit behavioral suite over the built stub
* `tools/` - helpers, utils and toolings
* `build/` - ephemeral build directory
* `build/tmp` - project-scoped temp directory
* `docker/` - reserved for containerized runs (currently empty)

## Source fidelity and reconstructed names

- Identifiers are English, ordinary Pascal capitalization
  (e.g. `ChooseRace`, `CheckpointStage`, `CheckLevelUp`); acronyms keep
  their conventional form (`DJ`). The user-facing string literals stay
  Polish - they are the game's observable output and are immutable.

## Commits

One line, conventional prefix (`docs: feat: fix: refactor: ci:` etc.),
no bodies. Concise and short messages.
Work lands on `dev`, one commit per functional change/subject. `main` is
fast-forwarded from `dev` only after maintainer confirmation; `main`
history is rewritten only on explicit demand.

## Text

LF, UTF-8, Polish diacritics preserved.
Files always end with a newline; enforced locally by `.githooks/pre-commit`
(`git config core.hooksPath .githooks`).

## String immutability & behavior preservation

**Strings are fixed.** No user-visible text (WriteLn messages, prompts,
item names, monster descriptions, level names, combat logs, etc.) may be
changed, reworded, reordered, or reformatted. The exact byte sequence of
every string literal in `src-pas/` - the reference source and the `Texts.inc`
catalog - is part of the behavioral contract;
`tests/strings_test.py` pins the union against
`tests/reference_strings.txt`.

**Behavior is immutable.** The refactor must produce *identical* observable
behavior for every possible input sequence:
- Terminal output (exact strings, order, formatting, timing-agnostic)
- Game state transitions (room changes, stat modifications, inventory)
- RNG-dependent outcomes (seeded via `BOMBKI_SEED` for reproducibility)
- Save/load round-trips (identical `pliki.tpu` content)

**Save file format preservation.** The binary save file (`pliki.tpu`) format
must remain byte-identical to the reference format stored in
`tests/reference_saves/pliki.tpu.ref`. A dedicated test
`tests/save_format_test.py` validates this by running a save scenario
and comparing the emitted `pliki.tpu` byte-for-byte against the stored
reference. This test must pass on the FPC reference (dev) build.

Refactoring is restricted to *internal* structure: control flow,
helper extraction, variable renaming, dead-code removal, constant
folding, and algorithmic simplification that does not alter any
observable result. Any change that causes a scenario or unit test to
diverge is a bug and must be reverted.
