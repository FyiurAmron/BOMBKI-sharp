namespace Bombki.Tests;

internal static class UnitsSeed {
    public static void Reseed() {
        Environment.SetEnvironmentVariable("BOMBKI_SEED", "0x9E3779B97F4A7C15");
        Game.Randomize();
    }
}
