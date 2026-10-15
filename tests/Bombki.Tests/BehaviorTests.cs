using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Bombki.Tests;

public sealed class BehaviorTests
{
    private const string Seed = "0x9E3779B97F4A7C15";
    private const int Level2Deduction = 725;

    public static TheoryData<string> Scenarios => new()
    {
        "level-up-deduction",
        "level-zero",
        "shop-item-display",
        "shop-repeat-buy",
        "shop-quirks",
        "race-stats-czlowiek",
        "race-stats-olbrzym",
        "race-stats-nimfa",
        "race-stats-pol-elf",
        "race-stats-ufok",
        "race-stats-czaromil",
        "zre-limits",
        "mode",
        "mode-save-load",
        "test_save_load_roundtrip",
        "mode-sleep",
        "ground-commands",
        "ground-skills",
        "shops-rich",
        "czaromil-checkpoints",
        "arena-poster-tour",
        "cage-fights-tour",
        "cages-tour",
        "concert-tour",
        "dark-street-tour",
        "quest-road-tour",
        "school-tour",
        "road-rooms",
        "city-dogs",
        "city-people",
        "cage-14-loot",
        "cage-14-loss",
        "cage-15-loot",
        "cage-15-loss",
        "cage-16-loot",
        "cage-16-loss",
        "cage-diploma",
        "cage-heart-drop",
        "room72-exit",
        "wyjsc0ie-typo",
        "living-door-escape",
        "living-door-defeat",
        "staruch-gifts",
        "well",
    };

    [Theory]
    [MemberData(nameof(Scenarios))]
    public void Scenario_matches_original_behavior(string name)
    {
        var root = FindRepoRoot();
        var binary = FindStubBinary(root);
        var scenario = root + "/tests/scenarios/" + name + ".json";
        Assert.True(File.Exists(scenario), $"scenario file not found: {scenario}");

        var logDir = root + "/build/pty-csharp";
        Directory.CreateDirectory(logDir);
        var log = logDir + "/" + name + ".jsonl";
        var harness = root + "/tests/expect_pty.py";

        var psi = new ProcessStartInfo("python3", $"\"{harness}\" \"{binary}\" \"{scenario}\" --log \"{log}\" --timeout 20")
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        psi.Environment["BOMBKI_SEED"] = Seed;

        using var process = Process.Start(psi);
        Assert.NotNull(process);
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(180_000))
        {
            try { process.Kill(entireProcessTree: true); } catch { }
            Assert.Fail($"FAIL {name}: harness timed out after 180s\nstdout:\n{stdout}\nstderr:\n{stderr}");
        }

        if (process.ExitCode != 0)
        {
            Assert.Fail($"FAIL {name}: expect_pty exited {process.ExitCode}\nstdout tail:\n{Tail(stdout, 2000)}\nstderr:\n{stderr}\nlog: {log}");
        }

        if (name == "level-up-deduction")
        {
            var reason = CheckLevelUpDeduction(log);
            if (reason is not null)
            {
                Assert.Fail($"FAIL {name}: {reason}");
            }
        }
    }

    private static string? CheckLevelUpDeduction(string logPath)
    {
        int? before = null;
        int? residue = null;
        var promptValue = new Regex(@"[0-9]+%\.(?<kunszt>-?[0-9]+)>", RegexOptions.Compiled);
        var gap = new Regex(@"BRAKUJE CI (?<gap>-?[0-9]+) KUNSZTU", RegexOptions.Compiled);

        foreach (var line in File.ReadLines(logPath))
        {
            JsonDocument doc;
            try { doc = JsonDocument.Parse(line); }
            catch { continue; }
            using (doc)
            {
                var element = doc.RootElement;
                if (!element.TryGetProperty("event", out var ev) || ev.GetString() != "MATCH")
                {
                    continue;
                }

                var label = element.TryGetProperty("label", out var l) ? l.GetString() ?? "" : "";
                var matched = element.TryGetProperty("matched", out var m) ? m.GetString() ?? "" : "";
                if (label.Contains("UNMODE to force the pass"))
                {
                    var mm = promptValue.Match(matched);
                    if (mm.Success && int.TryParse(mm.Groups["kunszt"].Value, out var k))
                    {
                        before = k;
                    }
                }

                if (label.Contains("sheet shows the level-2 line"))
                {
                    var mm = gap.Match(matched);
                    if (mm.Success && int.TryParse(mm.Groups["gap"].Value, out var g))
                    {
                        residue = g;
                    }
                }
            }
        }

        if (before is null || residue is null)
        {
            return "transcript lacks the leveling prompt or the level-2 sheet line";
        }

        var observed = before.Value + residue.Value - Level2Deduction;
        if (observed != Level2Deduction)
        {
            return $"prompt KUNSZT {before} + sheet residue {residue} implies a {observed} deduction at level 1, expected {Level2Deduction}";
        }

        return null;
    }

    private static string FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir, "BombkiSharp.slnx")) && File.Exists(Path.Combine(dir, "tests/expect_pty.py")))
            {
                return dir.TrimEnd('/');
            }

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new InvalidOperationException("repo root (BombkiSharp.slnx) not found");
    }

    private static string FindStubBinary(string root)
    {
        var names = OperatingSystem.IsWindows() ? new[] { "Bombki.exe" } : new[] { "Bombki", "Bombki.dll" };
        var configs = new[] { "Debug", "Release" };
        foreach (var config in configs)
        {
            foreach (var name in names)
            {
                var candidate = $"{root}/src/Bombki/bin/{config}/net11.0/{name}";
                if (File.Exists(candidate))
                {
                    if (name.EndsWith(".dll"))
                    {
                        continue;
                    }

                    return candidate;
                }
            }
        }

        throw new FileNotFoundException($"stub binary not found under src/Bombki/bin/*/net11.0/. Run 'dotnet build' first.");
    }

    private static string Tail(string text, int maxChars)
    {
        if (text.Length <= maxChars)
        {
            return text;
        }

        return text[^maxChars..];
    }
}
