---
name: ci
description: Pipelines and their results. Use when working with GitHub CI system.
---

# CI pipelines and runs

## The workflow

One workflow runs on every push to any branch, every pull request and on demand.
Four jobs:

- `style` - the style gate (e.g. formatting, data-file lint, JSON canonical
  form, text hygiene), then the data contract tests (e.g. `dotnet test`).
- `image` - the toolchain image (e.g. .NET SDK, Godot editor, export templates)
  is pushed to the container registry only when its exact tag is missing from
  the registry; otherwise the job just reports that it is there. The tag comes
  from the pinned versions, so a new toolchain pin builds a new image and an
  unchanged pin reuses the cached one.
- `build` - Linux and Windows exports inside that image, uploaded as full and
  minimal artifacts. It needs `style` and `image` but runs on `always()`, so a
  style failure must not block the exports.
- `release` - a rolling release on every push to main: one permanent `latest`
  release whose assets are replaced in place, so only the newest build is ever
  available. It publishes only when the build succeeded; a failed build
  publishes nothing and leaves the previous release untouched. Stale assets are
  pruned, the notes are dated (local time plus UTC), the `latest` tag is
  force-moved to the new commit, and uploads are retried because clobbering
  races the asset deletion.

## Reading a run

- Pipeline breakage is usually pipeline-specific: a red run that covers none of
  the changed files says little about them. Verify the change locally before
  trusting a red run.
- Never wait on a pipeline unless it is what you changed or your task depends on
  it; waiting on an unrelated run just burns time. Report the pipeline result
  when it lands.
