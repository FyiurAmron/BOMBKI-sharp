namespace Bombki.Tests;

// Pascal unit tests run one per process, so every test starts from the
// PRNG's fixed initial state. The xUnit port shares one process, so each
// randomness-consuming setup reseeds to the fixed default first: same
// position-zero draws for every test, no cross-test drift.
internal static class UnitsSeed
{
    public static void Reseed()
    {
        Environment.SetEnvironmentVariable("BOMBKI_SEED", "0x9E3779B97F4A7C15");
        Game.Randomize();
    }
}
