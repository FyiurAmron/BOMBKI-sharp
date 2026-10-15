using System.Runtime.CompilerServices;

namespace Bombki.Tests;

// In-process unit tests exercise full room/combat procedures whose
// Delays would otherwise sleep real time (the PTY harness instead
// passes BOMBKI_DELAY_MULT=0 to the spawned binary). Compress timing
// here the same way, unless explicitly overridden from outside.
internal static class AssemblySetup
{
    [ModuleInitializer]
    public static void Init()
    {
        if (Environment.GetEnvironmentVariable("BOMBKI_DELAY_MULT") is null)
            Environment.SetEnvironmentVariable("BOMBKI_DELAY_MULT", "0");
    }
}
