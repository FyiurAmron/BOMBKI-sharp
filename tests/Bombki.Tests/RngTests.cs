using System.Linq;

namespace Bombki.Tests;

// Pins the deterministic PRNG bit-for-bit against the test oracle
// (the FPC dev build's xorshift64* stub): same seed mapping, same
// reduction, Random(0) advancing nothing.
public sealed class RngTests
{
    private static short[] Draws(int bound, int count)
    {
        short[] draws = new short[count];
        for (int i = 0; i < count; i++)
            draws[i] = Game.Roll(bound);
        return draws;
    }

    [Fact]
    public void Rolls_match_oracle_sequence()
    {
        Environment.SetEnvironmentVariable("BOMBKI_SEED", "0x9E3779B97F4A7C15");
        Game.Randomize();
        Assert.Equal(new short[] { 74, 77, 46, 11, 93, 99, 36, 78, 86, 9 },
            Draws(100, 10));
    }

    [Fact]
    public void Roll_zero_returns_zero_without_advancing()
    {
        Environment.SetEnvironmentVariable("BOMBKI_SEED", "0x9E3779B97F4A7C15");
        Game.Randomize();
        short[] before = Draws(100, 3);
        Assert.Equal(new short[] { 74, 77, 46 }, before);
        Assert.Equal(0, Game.Roll(0));
        Assert.Equal(11, Game.Roll(100));
    }

    [Fact]
    public void Empty_seed_falls_back_to_default()
    {
        Environment.SetEnvironmentVariable("BOMBKI_SEED", "");
        Game.Randomize();
        short[] blank = Draws(100, 3);
        Environment.SetEnvironmentVariable("BOMBKI_SEED", "0x9E3779B97F4A7C15");
        Game.Randomize();
        Assert.Equal(Draws(100, 3), blank);
    }

    [Fact]
    public void Decimal_seed_parses()
    {
        Environment.SetEnvironmentVariable("BOMBKI_SEED", "12345");
        Game.Randomize();
        Assert.Equal(new short[] { 70, 79, 39 }, Draws(100, 3));
    }
}
