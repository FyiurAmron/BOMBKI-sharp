---
name: yaml
description: YAML parsing, formatting and linting. Use when editing data files.
---

# YAML formatting and usage

## Parsing contract

- Files are parsed through a single parser that keeps no naming convention: a
  key is the field name spelled exactly, as it is in the model (a file says
  `Name` because the model's field is `Name`). No translation layer, no case
  mapping, nothing to keep in sync.
- Unknown keys are an error, so a typo fails the load instead of leaving a
  default behind. A missing key leaves the field default. Add a field, not a
  reader: one parse call is the whole load.
- Keys that are ids rather than fields stay lowercase: direction keys, entity
  ids as map keys. They are content, not shape.
- The YAML null family (`""`, `~`, `null`, `Null`, `NULL`) all read as the same
  sentinel; `~` marks a default in the data.
- A block scalar (`Notes: |`) keeps one line break past the last line; the
  loader trims it on load.

## Format

- Pretty-printed: 2-space indent, one entry per line. Block style is the default
  for collections; flow style is allowed wherever it reads better (`Map: [2, 5,
  0]`).
- Sibling keys align their values to one column: pad the shorter keys with
  spaces, so the eye scans a straight edge instead of a ragged one.

  ```yaml
  star:    "\\u2605"
  diamond: "\\u25c6"
  ```

- Alignment tooling must be structure-aware: skip block-scalar content (prose
  like `Relax: it is only...` reads as a key to a naive regex) and leave
  empty-value parent keys bare (padding them would be trailing whitespace).
- Data files carry no comments: no `#` lines, no `_note`-style keys, no prose of
  any kind. A data file is values only. What a field or a file means belongs in
  the docs or in the reader that parses it, never in the data.

## Linting

- A lint tool parses every data and config file and fails on a parse error, a
  tab, or odd indentation. Run it directly or through a style gate.
