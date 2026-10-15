using System.Collections.Generic;

// Minimal stand-in for the engine's list extensions, which the lint
// uses in one place. The Godot engine project supplies the real ones.
static class ListExtensions {
    public static string JoinToString(this List<string> items) =>
        string.Join(", ", items);
}
