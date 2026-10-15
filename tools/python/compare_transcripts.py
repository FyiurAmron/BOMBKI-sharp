#!/usr/bin/env python3
"""Compare two raw PTY transcripts for game-behavior identity.

Timing artifacts are normalized away because they differ run to
run even on one binary: the startup TICK count (a scheduling race
at the first KeyPressed check), the FPC runtime init prefix (not
game code), the presend echo of deterministic drivers, ANSI cursor
motion (proven byte-exact separately), and newline flavor including
the scroll branch (screen-fill state follows the TICK count) plus
the empty-read echo rendering (a bare newline versus ESC[1B versus
a scroll newline, all depending on that same screen state).

What remains - every printed text line in order, including all
numbers - must match exactly. The game never prints a meaningful
empty line, so those are dropped rather than aligned.
"""

import re
import sys
from pathlib import Path

RUNTIME_PREFIX = b"\x1b[6n\x1b[H\x1b[m"
ANSI = re.compile(rb"\x1b\[[0-9;?]*[A-Za-z]|\x1b[()][0-9A-B]|\x1b=|\x1b>")


def read_bytes(path):
    """Raw transcript bytes, or OUTPUT raw_hex events of a harness log."""
    raw = Path(path).read_bytes()
    stripped = raw.lstrip()
    if stripped.startswith(b"{"):
        import json
        out = ""
        for line in raw.decode("utf-8").splitlines():
            line = line.strip()
            if not line:
                continue
            event = json.loads(line)
            if event.get("event") == "OUTPUT":
                out += event.get("raw_hex", "")
        return bytes.fromhex(out)
    return raw


def content_lines(raw: bytes) -> list:
    if raw.startswith(RUNTIME_PREFIX):
        raw = raw[len(RUNTIME_PREFIX):]
    # Presend echo of deterministic drivers (a lone leading newline).
    if raw.startswith(b"\r\n"):
        raw = raw[2:]
    text = ANSI.sub(b"", raw).decode("cp437")
    lines = re.split(r"\r\n|\n\r|\n|\r", text)
    return [line for line in lines
            if line.strip() not in ("", "TICK !!!")]


def main() -> int:
    ref = content_lines(read_bytes(sys.argv[1]))
    got = content_lines(read_bytes(sys.argv[2]))
    print(f"lines: {len(ref)} ref vs {len(got)} got")
    if ref == got:
        print("IDENTICAL")
        return 0
    total = min(len(ref), len(got))
    first = next((i for i in range(total) if ref[i] != got[i]), None)
    if first is None:
        longer, name = (ref, "ref") if len(ref) > len(got) else (got, "got")
        print(f"DIVERGE: {name} has {abs(len(ref) - len(got))} extra lines from {total}:")
        for line in longer[total:total + 4]:
            print(f"  {name}: {line[:120]!r}")
        return 1
    print(f"DIVERGE at line {first}:")
    for i in range(max(0, first - 1), min(total, first + 2)):
        mark = ">>>" if i == first else "   "
        print(f"  {mark} ref: {ref[i][:120]!r}")
        print(f"  {mark} got: {got[i][:120]!r}")
    return 1


if __name__ == "__main__":
    sys.exit(main())
