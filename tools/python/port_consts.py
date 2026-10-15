#!/usr/bin/env python3
"""Translate the Pascal string/integer const sections to literal C#.

Reads src-pas/refactored/Texts.inc into src/Bombki/Texts.cs and the room
numbers of src-pas/refactored/Rooms.inc into the const block at the top of
src/Bombki/Rooms.cs, preserving every byte: Pascal `#NNN` codes and any
UTF-8 source bytes become explicit U+00XX escapes, decoded at runtime 1:1
through Latin-1. String arrays keep their 1-based bounds via an unused
index 0 so `Low`/`High` loops translate without renumbering.
"""

import re
import sys
from pathlib import Path

PROJECT = Path(__file__).resolve().parents[1]
SRC_PAS = PROJECT / "src-pas" / "refactored"
OUT = PROJECT / "src" / "Bombki" / "Texts.cs"
ROOMS_OUT = PROJECT / "src" / "Bombki" / "Rooms.cs"
ROOMS_HEADER = "static class Rooms\n{\n"
ROOM_CONST_LINE = re.compile(r"    public const int \w+ = -?\d+;$")

STRING_RE = re.compile(r"'((?:[^']|'')*)'|#(\d+)")


def parse_string_expr(expr):
    """Split a Pascal string expression into literal/code chunks."""
    chunks = []
    pos = 0
    for match in STRING_RE.finditer(expr):
        if match.start() != pos:
            raise ValueError(f"unparsed text in string expr: {expr!r}")
        lit, code = match.groups()
        if lit is not None:
            chunks.append(lit.replace("''", "'"))
        else:
            chunks.append(chr(int(code)))
        pos = match.end()
    if pos != len(expr.strip()):
        raise ValueError(f"trailing text in string expr: {expr!r}")
    return "".join(chunks)


def cs_escape(text):
    """Escape text to a C# literal preserving exact source bytes."""
    out = []
    for char in text:
        code = ord(char)
        if char == "\\":
            out.append("\\\\")
        elif char == '"':
            out.append('\\"')
        elif code < 128:
            if code < 32:
                out.append(f"\\u{code:04X}")
            else:
                out.append(char)
        elif code < 256:
            out.append(f"\\u{code:04X}")
        else:
            for byte in char.encode("utf-8"):
                out.append(f"\\u{byte:04X}")
    return "".join(out)


def decl_depth(text):
    """Paren depth of text, ignoring parens inside string literals."""
    depth = 0
    i, n = 0, len(text)
    while i < n:
        char = text[i]
        if char == "'":
            if text[i + 1:i + 2] == "'":
                i += 2
                continue
            i += 1
            while i < n:
                if text[i] == "'":
                    if text[i + 1:i + 2] == "'":
                        i += 2
                        continue
                    break
                i += 1
            i += 1
        elif char == "#":
            while i < n and (text[i] == "#" or text[i].isdigit()):
                i += 1
        else:
            if char == "(":
                depth += 1
            elif char == ")":
                depth -= 1
            i += 1
    return depth


def gather_decls(path):
    """Yield decls, joining multiline arrays."""
    text = path.read_text(encoding="utf-8")
    buffer = ""
    depth = 0
    for line in text.splitlines():
        stripped = line.strip()
        if not stripped or stripped == "const":
            continue
        buffer += " " + stripped
        depth += decl_depth(stripped)
        if buffer.rstrip().endswith(";") and depth <= 0:
            yield buffer.strip()[:-1].strip()
            buffer = ""
            depth = 0
    if buffer.strip():
        raise ValueError(f"leftover text in {path}: {buffer!r}")


def parse_decls(name):
    """Split one include's declarations into (consts, string arrays)."""
    consts = []
    arrays = []
    for decl in gather_decls(SRC_PAS / name):
        match = re.match(r"(\w+)\s*:\s*array\[(\d+)\.\.(\d+)\]\s*of\s*string\s*=\s*\((.*)\)$",
                         decl, re.S)
        if match:
            ident, lo, hi, items = match.groups()
            parts = [p.strip() for p in items.split(",")]
            if len(parts) != int(hi) - int(lo) + 1:
                raise ValueError(f"bound mismatch: {decl!r}")
            rendered = []
            for part in parts:
                if part.startswith("'") or part.startswith("#"):
                    rendered.append(f'"{cs_escape(parse_string_expr(part))}"')
                else:
                    rendered.append(part)
            arrays.append((ident, int(lo), rendered))
            continue
        match = re.match(r"(\w+)\s*=\s*(-?\d+)$", decl)
        if match:
            consts.append((match.group(1), match.group(2), "int"))
            continue
        match = re.match(r"(\w+)\s*=\s*(.+)$", decl, re.S)
        if match:
            ident, expr = match.groups()
            consts.append((ident, f'"{cs_escape(parse_string_expr(expr.strip()))}"', "string"))
            continue
        raise ValueError(f"unhandled decl: {decl!r}")
    return consts, arrays


def write_rooms_consts(consts):
    """Replace the room const block under the Rooms.cs class header."""
    text = ROOMS_OUT.read_text(encoding="utf-8")
    at = text.find(ROOMS_HEADER)
    if at < 0:
        raise ValueError(f"{ROOMS_OUT} lost its class header")
    prefix = text[:at + len(ROOMS_HEADER)]
    lines = text[at + len(ROOMS_HEADER):].split("\n")
    index = 0
    while index < len(lines) and ROOM_CONST_LINE.match(lines[index]):
        index += 1
    while index < len(lines) and lines[index] == "":
        index += 1
    block = [f"    public const int {ident} = {value};" for ident, value, _ in consts] + [""]
    ROOMS_OUT.write_text(prefix + "\n".join(block + lines[index:]),
                         encoding="utf-8", newline="\n")


def main():
    consts, arrays = parse_decls("Texts.inc")

    lines = ["// Literal port of src-pas/refactored/Texts.inc and src-pas/refactored/Rooms.inc.",
             "// String bytes are preserved exactly; see tools/port_consts.py.",
             "",
             "public static partial class Game",
             "{"]
    for ident, value, kind in consts:
        if kind != "string":
            raise ValueError(f"unexpected int const in Texts.inc: {ident}")
        lines.append(f"    public const string {ident} = {value};")
    for ident, lo, parts in arrays:
        assert lo == 1, f"unexpected lower bound in {ident}"
        lines.append(f"    public static readonly string[] {ident} = {{ \"\", {', '.join(parts)} }};")
    lines += ["}", ""]
    OUT.write_text("\n".join(lines), encoding="utf-8", newline="\n")
    print(f"wrote {OUT} ({len(consts)} consts, {len(arrays)} arrays)")

    rooms_consts, rooms_arrays = parse_decls("Rooms.inc")
    if rooms_arrays:
        raise ValueError(f"unexpected string arrays in Rooms.inc: {rooms_arrays}")
    for ident, _, kind in rooms_consts:
        if kind != "int":
            raise ValueError(f"unexpected string const in Rooms.inc: {ident}")
    write_rooms_consts(rooms_consts)
    print(f"wrote {ROOMS_OUT} ({len(rooms_consts)} room consts)")


if __name__ == "__main__":
    sys.exit(main())
