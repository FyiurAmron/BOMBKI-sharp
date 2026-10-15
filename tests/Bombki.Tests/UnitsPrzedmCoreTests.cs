using static Game;

namespace Bombki.Tests;

public class UnitsPrzedmCoreTests {
    [Fact]
    public void Mode() {
        Session.Room = 42;
        EnterTrance();
        Assert.Equal(42, Session.OuterRoom);
        Assert.Equal(1000, Session.Room);
    }

    [Fact]
    public void Komendy() {
        Command = "PN";
        ExpandAbbrev();
        Assert.Equal("POLNOC", Command);
        Command = "PD";
        ExpandAbbrev();
        Assert.Equal("POLODNIE", Command);
        Command = "W";
        ExpandAbbrev();
        Assert.Equal("WSCHOD", Command);
        Command = "Z";
        ExpandAbbrev();
        Assert.Equal("ZACHOD", Command);
        Command = "G";
        ExpandAbbrev();
        Assert.Equal("GORA", Command);
        Command = "D";
        ExpandAbbrev();
        Assert.Equal("DOL", Command);
        Command = "E";
        ExpandAbbrev();
        Assert.Equal("EXIT", Command);
        Command = "M";
        ExpandAbbrev();
        Assert.Equal("MODE", Command);
        Command = "POLNOC";
        ExpandAbbrev();
        Assert.Equal("POLNOC", Command);
    }

    [Fact]
    public void TrainSila() {
        Player.Stats[(int)TStat.Strength] = 10;
        Player.Stats[(int)TStat.MaxStrength] = 20;
        Player.Practice = 10;
        Command = "TRENUJ SILA";
        HandleTrain();
        Assert.Equal(11, Player.Stats[(int)TStat.Strength]);
        Assert.Equal(7, Player.Practice);
    }

    [Fact]
    public void TrainZrecznosc() {
        Player.Stats[(int)TStat.Dexterity] = 10;
        Player.Stats[(int)TStat.MaxDexterity] = 20;
        Player.Practice = 10;
        Command = "TRENUJ ZRECZNOSC";
        HandleTrain();
        Assert.Equal(11, Player.Stats[(int)TStat.Dexterity]);
        Assert.Equal(8, Player.Practice);
    }

    [Fact]
    public void TrainMadrosc() {
        Player.Stats[(int)TStat.Wisdom] = 10;
        Player.Stats[(int)TStat.MaxWisdom] = 20;
        Player.Practice = 10;
        Command = "TRENUJ MADROSC";
        HandleTrain();
        Assert.Equal(11, Player.Stats[(int)TStat.Wisdom]);
        Assert.Equal(7, Player.Practice);
    }

    [Fact]
    public void TrainNeedsPracticePoints() {
        Player.Stats[(int)TStat.Strength] = 10;
        Player.Stats[(int)TStat.MaxStrength] = 20;
        Player.Practice = 2;
        Command = "TRENUJ SILA";
        HandleTrain();
        Assert.Equal(10, Player.Stats[(int)TStat.Strength]);
        Assert.Equal(2, Player.Practice);
    }

    [Fact]
    public void TrainCapsAtMaximum() {
        Player.Stats[(int)TStat.Strength] = 20;
        Player.Stats[(int)TStat.MaxStrength] = 20;
        Player.Practice = 10;
        Command = "TRENUJ SILA";
        HandleTrain();
        Assert.Equal(20, Player.Stats[(int)TStat.Strength]);
        Assert.Equal(10, Player.Practice);
    }

    [Fact]
    public void PotworyRanges() {
        UnitsSeed.Reseed();
        SpawnMonsters();
        List<TMonster> beasts = [
            TMonster.Bacterium, TMonster.Snail, TMonster.Beetle,
            TMonster.Cockroach, TMonster.Ant, TMonster.Spider,
            TMonster.Boar, TMonster.Fox, TMonster.Rat,
            TMonster.Partridge, TMonster.Hare, TMonster.Mongrel,
            TMonster.Eagle, TMonster.Deer, TMonster.Elephant,
            TMonster.Lion, TMonster.Giraffe, TMonster.Camel,
            TMonster.Ostrich, TMonster.Boa, TMonster.Wolf,
            TMonster.Bison, TMonster.Panther, TMonster.Gladiator,
            TMonster.Warrior, TMonster.Tamer,
        ];
        foreach (TMonster m in beasts)
            Assert.InRange(World.Monsters[(int)m], 33, 57);
        Assert.InRange(World.Monsters[(int)TMonster.Kid], 61, 66);
        Assert.InRange(World.Monsters[(int)TMonster.Reporter], 61, 66);
        List<TMonster> maniacs = [
            TMonster.Lunatic, TMonster.Listener, TMonster.Fan,
            TMonster.Man, TMonster.OldMan,
        ];
        foreach (TMonster m in maniacs)
            Assert.InRange(World.Monsters[(int)m], 61, 67);
        List<TMonster> heavies = [
            TMonster.Policeman, TMonster.Bouncer, TMonster.Gorilla,
        ];
        foreach (TMonster m in heavies)
            Assert.InRange(World.Monsters[(int)m], 61, 68);
        List<TMonster> musicians = [
            TMonster.Guitarist, TMonster.Drummer, TMonster.Keyboardist,
            TMonster.Liroy,
        ];
        foreach (TMonster m in musicians)
            Assert.InRange(World.Monsters[(int)m], 70, 72);
        Assert.Equal(67, World.Monsters[(int)TMonster.Minibarman]);
        Assert.Equal(67, World.Monsters[(int)TMonster.Fatso]);
        Assert.Equal(69, World.Monsters[(int)TMonster.DJ]);
        Assert.Equal(76, World.Monsters[(int)TMonster.Pedal]);
        Assert.Equal(76, World.Monsters[(int)TMonster.Couple]);
        Assert.Equal(76, World.Monsters[(int)TMonster.Maciek]);
        Assert.Equal(1, World.Door);
        Assert.Equal(1, World.OldElf);
        List<TMonster> dogs = [
            TMonster.Dachshund, TMonster.Shepherd, TMonster.Pooch,
            TMonster.Spaniel, TMonster.Poodle, TMonster.Cabman,
            TMonster.Vendor, TMonster.Sweeper, TMonster.Drunkard,
            TMonster.SickMan,
        ];
        foreach (TMonster m in dogs)
            Assert.InRange(World.Monsters[(int)m], 20, 26);
        List<TMonster> flowers = [
            TMonster.Sorrel, TMonster.Daisy, TMonster.Clover,
            TMonster.SowThistle, TMonster.Dandelion, TMonster.Rose,
            TMonster.Thistle, TMonster.Raspberry,
        ];
        foreach (TMonster m in flowers)
            Assert.InRange(World.Monsters[(int)m], 77, 83);
        List<TMonster> berries = [
            TMonster.Gooseberry, TMonster.Blackberry, TMonster.Grass,
            TMonster.Duncan,
        ];
        foreach (TMonster m in berries)
            Assert.InRange(World.Monsters[(int)m], 77, 84);
    }

    [Fact]
    public void TarczaAppliesShield() {
        UnitsSeed.Reseed();
        Combat.Damage = 50;
        Gear.Values[(int)TGearItem.WornCount] = 30;
        Gear.Values[(int)TGearItem.Armor] = 100;
        ShieldBlock();
        Assert.Equal(20, Combat.Damage);
    }

    [Fact]
    public void TarczaClampsAtZero() {
        UnitsSeed.Reseed();
        Combat.Damage = 10;
        Gear.Values[(int)TGearItem.WornCount] = 30;
        Gear.Values[(int)TGearItem.Armor] = 100;
        ShieldBlock();
        Assert.Equal(0, Combat.Damage);
    }
}
