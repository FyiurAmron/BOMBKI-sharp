---
name: cs
description: C# style and patterns. Use when writing or reviewing C#.
---

# C# style and patterns

## Style as practiced

- Prefer the newest C# form that fits the language version in use.
- No comments, no summaries. Executable code should be self-documenting.
  Only empty line comments are allowed, used as forced line-breaks.
- One statement per line, even when the language allows more. Opening braces go
  on the same line as the statement that opens them; an opening brace never
  stands on a line of its own.
- A helper used twice is worth generalizing.
- A literal used twice is a `const`.
- Never emit the default access modifier: omit `private`; keep `public` where it
  changes the default.
- Never seal types: classes stay open for extension.
- Dispatch is a switch expression with `Action` arms, invoked once at the end -
  not a `case`/`break` chain:

  ```csharp
  Action step = kind switch {
      Text => () => Show(payload[0]),
      State => Refresh,
      _ => null
  };
  step?.Invoke();
  ```

- Prefer functional composition (`Select`/`Where`/
  `FirstOrDefault`/`Any`/`ToDictionary`/`AddRange` with small reader functions)
  over manual loops. Loops stay where they draw, mutate frame state, or
  accumulate. An empty collection filled by one loop over one source is a
  `Select` (or `ToDictionary`, or a spread), not a loop with `Add`.
- Use a method group wherever a lambda only wraps one call: `ForEach(Say)`
  beats `ForEach(line => Say(line))`.
- Small methods take a lambda body (`=>`), not a block.
- Start the body of a standalone `=>` method on the line after the arrow.
- No `partial` types unless external tooling needs them.
- Repeated calls of one function mean one collection and a loop over it: a list
  when the call takes one varying argument, a map when it takes two.
- A clean if/else pair that only chooses a value is a ternary.
- `else` and `catch` hug the closing brace (`} else {`), K&R style.
- Multi-line strings use verbatim interpolated blocks (`$@"..."`), not per-line
  calls.
- Compose strings with interpolation (`$"..."`), never `+` concatenation.
- Collections are `List<>` with collection expressions, never arrays.
- Create collections with a collection initializer (`[...]`) instead of `new`
  wherever the type allows it.
- Use target-typed `new()`: never repeat a type the compiler already knows.
  Prefer `Config r = new();` over `var r = new Config();` - shorter and more
  readable: the type leads, and the compiler already knows it.
- Two or more declarations of one type share a single type mention with
  comma-separated declarators, one per line, and a bare `//` after the type. The
  `//` is not a comment: it blocks auto-reformatting. Declarations only fold
  when adjacent; when another type sits between them, the order carries meaning
  and stays. Trap: C# comma declarators do not work for expression-bodied
  members (CS1002/CS1519) - group fields, not expression-bodied properties.

  ```csharp
  static readonly Option //
      Amber = new(1f, 0.71f, 0.28f),
      Phosphor = new(0.55f, 1f, 0.62f);
  ```

- Set properties in the initializer, not after construction: `new() { VolumeDb =
  -6 }` beats `new(); p.VolumeDb = -6;`.
- Namespaces have to be file-scoped.
- Use static imports for commonly used functions (`using static ...`)
  instead of qualifying every call.
- Multi-line lists end with a trailing comma.
- Prefer `.Any()` to `.Count > 0` for an existence check.
- One flow per LINQ chain: no throwaway variable that splits a query into a
  check plus a later consumer.

## Error handling

- Error messages terse. No trailing period: it obscures the collection. The message is the
  problem, " : ", and the items joined.
- Never let the sad path run on the happy path. The happy path is what executes
  every time, so build the error (join, format, allocate) only after the check
  has already failed: test with `.Any()`, then do the work.
- Do not catch exceptions that should propagate: fail fast on corrupt data and
  real bugs. Use explicit checks (existence, null, validation) for expected
  conditions; reserve graceful fallbacks for non-load-bearing extras (a missing
  optional asset falls back to a default).

## Extensions

- Prefer an extension that names the operation and carries the default: the
  subject leads the call instead of sitting among the arguments.
- Leverage the provided common extensions (`CommonExtensions.cs`): use
  `Times` for plain repetition, with or without a simple counter from 0
  to n. Spell an explicit range `(0..n).Indexes()` only for non-trivial
  cases, where the indices feed a longer chain. In more complex cases, use
  `Select` or `ForEach` instead of `for` loops if possible; likewise, use
  `Select` or `ForEach` instead of `foreach` loops.
- All extension methods live in one file per subject, named
  `<Subject>Extensions` (`StringExtensions`, `ExceptionExtensions`, ...). The
  subject is what the extension is about: the string it builds, the exception it
  throws.

## Testing

- Never add production code that exists only so a test can reach it; drive the
  real thing from the outside instead.
- Contract tests load the real files through the real parser; add a test when
  you add a file or a field.
- Test locals take `var` wherever the initializer already states the type
  (`Parse<List<ItemDef>>(...)`, a cast, a method chain): restating it on the
  left only repeats what the right-hand side just said. Complex types the
  initializer does not name (`Dictionary<string, string> m = new() {...}`)
  stay spelled out, with target-typed `new()`.
- Fold repeated calls into one collection (list/map) plus a single Assert.All
  or functional loop over that collection.
