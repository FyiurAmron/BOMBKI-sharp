---
name: repo
description: Instructions on using version control system repositories. Use when working with git.
---

# Repo conventions

## Git habits

- Never delete an untracked file unless explicitly asked to.
- Work happens on a working branch (say `dev`): commit and push there. A feature
  branch exists only when the user asks for one. Main merges from the working
  branch after manual approval, and merged branches are deleted locally and
  remotely.
- A git hook is the best way to enforce text hygiene: every text file ends with
  a newline and carries no trailing whitespace. Enable one once per clone by
  pointing `core.hooksPath` at a hooks directory.
- Squash fixups into the commit they fix; never leave the build broken in
  a commit when the fix is at hand.

## Commit messages

- One short line, no body, in Conventional Commits style: `type: description`
  (`fix:`, `feat:`, `docs:`, `chore:`; the log also uses `build:`, `style:`,
  `refactor:` and `ci:`), ASCII only. Ordinary capitalization applies: the
  type tag and common words stay lowercase, proper nouns keep their capitals
  (`API`, `GitHub`, `Linux`, `C#`).
- The line tells what, not how. Implementation details are not important for
  commit messages: the log is a changelog of changes, not a design document. The
  diff carries the how - which structure, which call, which guard - and the
  message carries the what: what differs for a reader of the log.
- Tell only what is unobvious; do not tell the obvious or the expected. A help
  text is expected to cover every command, so the coverage does not belong in
  the message that adds it; a default reply is per-key by definition, so the
  keying does not either. The line is short by rule, and the obvious spends it
  without informing the reader.
- Use the codebase's own vocabulary, and keep the line inside the length range
  its log already uses.

## Worked examples

- `feat: load the hint text from config` - the what: the text is data now, read
  from a file. Rejected forms: `... checked at init` and `... check every hint
  at init` name the mechanism; `... covering every hint` names the expected.
- `refactor: key the default replies` - the what: the replies are held in a map
  now. Rejected: `... throw on unknown keys` names the mechanism (the caller
  already records the same error as before), and `... by key` names the obvious
  keying.

## The gate

Run the whole gate from the repo root before reporting done - compile, style,
data tests, playthrough - not just the check that covers the change. A green
subset certifies only itself.

Wipe the build output before the build when the result must be trusted from
scratch: a gate must certify the current source, not the last compiled state.
Wiped after the build, the playthrough has nothing to run; wiped before it, the
build proves the source.

- Compile every project (e.g. `dotnet build` the solution).
- The style gate - e.g. formatting, data-file lint, JSON canonical form, text
  hygiene.
- The data contract tests (e.g. `dotnet test`): every data file parses into its
  type.
- The scripted playthrough of the built game (e.g. `godot --headless --path .`).
