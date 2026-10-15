using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bowcki.Engine;

var roots = args.Length > 0 ? args : ["data", "config"];
var fail = 0;

roots.ForEach(root => {
    if (!Directory.Exists(root)) {
        Console.WriteLine($"    missing directory: {root}");
        fail = 1;
        return;
    }

    Directory.EnumerateFiles(root, "*.yaml", SearchOption.AllDirectories).Order().ForEach(path => {
        string text = File.ReadAllText(path);
        Shape(text).ForEach(problem => {
            Console.WriteLine($"    {path}: {problem}");
            fail = 1;
        });

        object graph;
        try {
            graph = NanoYaml.ParseRaw(text);
        } catch (Exception e) {
            Console.WriteLine($"    {path}: parse error: {e.Message}");
            fail = 1;
            return;
        }

        NoteKeys(graph).ForEach(key => {
            Console.WriteLine($"    {path}: note key '{key}' - data files carry no prose");
            fail = 1;
        });
    });
});

return fail;

static IEnumerable<string> Shape(string text) {
    if (text.Contains('\r')) {
        yield return "CR in file, use LF line endings";
    }

    if (!text.EndsWith("\n")) {
        yield return "no newline at end of file";
    }

    var lines = text.Split('\n');
    for (int i = 0; i < lines.Length; i++) {
        string line = lines[i];
        int indent = line.Length - line.TrimStart(' ').Length;
        if (indent % 2 != 0) {
            yield return $"odd indentation on line {i + 1}";
        }

        if (line.Contains('\t')) {
            yield return $"tab on line {i + 1}";
        }

        if (line.TrimStart(' ').StartsWith('#')) {
            yield return $"comment on line {i + 1} - data files carry no comments";
        }
    }
}

static IEnumerable<string> NoteKeys(object node) {
    switch (node) {
        case IDictionary dict:
            foreach (DictionaryEntry entry in dict) {
                if (entry.Key is string key) {
                    if (key.StartsWith('_')) {
                        yield return key;
                    }

                    foreach (string nested in NoteKeys(entry.Value!)) {
                        yield return nested;
                    }
                }
            }
            break;
        case IEnumerable list:
            foreach (object item in list) {
                foreach (string nested in NoteKeys(item!)) {
                    yield return nested;
                }
            }
            break;
    }
}
