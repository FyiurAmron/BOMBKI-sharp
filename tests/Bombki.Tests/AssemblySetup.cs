using System.Runtime.CompilerServices;

namespace Bombki.Tests;

internal static class AssemblySetup {
    [ModuleInitializer]
    public static void Init() {
        if (Environment.GetEnvironmentVariable("BOMBKI_DELAY_MULT") is null)
            Environment.SetEnvironmentVariable("BOMBKI_DELAY_MULT", "0");
    }
}
