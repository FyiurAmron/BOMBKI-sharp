---
name: docs
description: Docs and their formats. Use when writing docs or skills.
---

# Documentation and skills

## What lives where

- CONTRIBUTING.md holds the contributor rules. It stays short, implicit and
  lecture-free: state the rule, not the reasoning, and do not spell out what the
  wording already implies - `etc.` is an open list, "YAML is primary" already
  allows JSON. A rule that needs a caveat to be applied correctly is worded
  badly.
- AGENTS.md holds the agent workflow and the skill inventory. It is read at
  session start, so it stays small: pointers and conduct, no reasoning.
- The skills (`_skills/<name>/SKILL.md`) hold the explicit knowledge: rationale,
  traps, the reasoning behind a convention, worked examples.
- CONTRIBUTING.md is the source of truth. A skill explains a rule; it never
  overrides one.

## Skill format

- One directory per skill, named for its subject, holding one `SKILL.md`. The
  file opens with YAML frontmatter - `name` and `description`, the standard
  skill metadata. The name is lowercase letters, numbers and hyphens; the
  description says what the skill does and when to use it, in one line - it is
  what matches a request to the skill.
- A skill covers its subject and nothing else: only rules reasonably applicable
  to every project of that kind. Project architecture, project-specific tools
  and the obvious stay out, and a rule another skill's subject owns stays in
  that skill.
- State every rule in its general form: applicable to any project of the kind,
  with no project-specific customization, no distinct class and no one-off
  implementation. A section built around one implementation is a tutorial for
  it, not a rule - keep the rule, folded into a general section, and drop the
  implementation. Reasoning: a skill travels to any project; an implementation
  stays where it was written.
- A skill is not a catalog of patterns: there are too many to fit. It holds the
  rules that decide the style, not every idiom for a case.
- A rule that is vague or easy to misinterpret is useless. Word it so two
  readers cannot apply it differently.
- A skill is standalone within its subject: it carries all the info needed to
  apply its rules, reasoning and worked examples included. The two rules compose
  - "its rules" is the set its subject owns, so completeness never demands a
  fact from another subject, and single ownership never leaves a rule short.
- Example names are invented; no real project is referenced. Subject vocabulary
  stays: language keywords, framework APIs, tool commands.
- No suppositions: a claim that the host project has something - a style gate, a
  git hook - might be false. State the rule, suggest the mechanism, or drop the
  claim.
- A skill never references another skill - no "see the X skill" - because the
  reader may have loaded none of them. The inventory routes by subject instead.

## Markdown hygiene

- Docs wrap at 80 columns (soft, reported) and stay under 120 (hard); fenced
  code is exempt from the soft limit.
- The 80-column wrap is a target, not a ceiling: a line that stops at 40 is
  compliant and useless. The wrap buys a steady right edge; a line that stops
  far short wastes the width. Fill every line to its last fitting word.
- Every text file ends with a newline and carries no trailing whitespace, and
  the repo is ASCII only: prose uses `-` where a dash is needed and `...`
  instead of an ellipsis.
- A style gate is the best way to enforce all of it: it reports the soft limit
  and fails the hard one.

## Worked example

The skills themselves: each opens with frontmatter, fills every line to 80,
stays in its subject, states its rules in their general form, holds rules not
patterns, words them so they cannot be misread, invents its example names,
supposes nothing, and mentions no other skill.
