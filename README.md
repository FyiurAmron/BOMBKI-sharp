*Dedicated to my children, today's and future; let The Sun always shine in your eyes.*

# BOMBKI-sharp

This is a test-backed rewrite of the reconstructed original 1999' BOMBKI text
RPG game. The goal is to use the test-backed Pascal refactoring of the original
code to create a complete minimal C# implementation - feature-identical
(including bugs). Bugfix release will follow after porting project is complete.

It is based on extensive work done as part of collaboration on
https://github.com/rzuf79/bombki , with an explicit approval from the game's
original author, Mateusz Pawluczuk. The test setup is based on extensive work
done in https://github.com/FyiurAmron/BOMBKI-tests (historic reference only now)
and the refactorings are based on extensive work done in
https://github.com/FyiurAmron/BOMBKI-refactor (also a historic reference).

tl;dr question:
>(...) uwaga, jest prosba o blogoslawienstwo \[projektu rekonstrukcyjnego\]!(...)

tl;dr answer:
>(...) Blogoslawienstwo (...) dane! (...) Pozdrawiam, Mateusz

## Setup

- .NET 11 SDK for building and testing the C# port (`dotnet build`,
  `dotnet test`).
- Python 3 for the analysis, build, and conformance scripts.
- Optional `pyte` (`python3 -m pip install pyte`) for screen-aware PTY tests
  with `tests/expect_pty.py --pyte`.
- Free Pascal (FPC) for building the frozen Pascal reference in `src-pas/`
  (`python3 tools/build_dev_game.py`, `tests/pas-conformance.sh`).

## TODO
- behavioral (external) test rewrite
- unit (internal) test rewrite
- code rewrite (using test results as ground truth)
- bugfixing
