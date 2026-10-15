using static Game;

namespace Bombki.Tests;

// Port of tests/units/test_swiat.pas: room procedures driven with
// scripted stdin (TestStdin pipe, mirroring the Pascal FeedStdin),
// asserting room state instead of terminal output.
public sealed class UnitsSwiatTests
{
    private static TestStdin FeedStdin(string lines) => TestStdin.Feed(lines);

    private static void RoomSetup()
    {
        Player.Energy = 30000; Player.Experience = 100;
        Player.Stats[(int)TStat.Strength] = 127;
        Player.Stats[(int)TStat.Dexterity] = 127;
        Combat.Fled = 0; Skills.Values[(int)TSkill.Fleeing] = 0;
        Skills.Values[(int)TSkill.FleeEnergy] = 0; Player.Mana = 0;
        Skills.Values[(int)TSkill.Parry] = 0;
        Combat.FireballCharges = 0; Combat.PoisonCharges = 0;
        Combat.PoisonRounds = 0; Skills.Values[(int)TSkill.Cooking] = 0;
        Skills.Values[(int)TSkill.Kick] = 0;
        Skills.Values[(int)TSkill.KickMinMana] = 0;
        Skills.Values[(int)TSkill.KickMaxEnergy] = 0;
        Gear.Values[(int)TGearItem.Armor] = 0;
        Gear.Values[(int)TGearItem.WornCount] = 0;
        Gear.Values[(int)TGearItem.LuckRoll] = 0;
        Player.MaxEnergy = 50;
        World.Monsters[(int)TMonster.CageStrong] = 0;
        Bag.Count = 0; World.Quest = 0; World.QuestKills = 0;
        Player.Money = 0;
        Bag.Permit = 0;
        Bag.Items[(int)TBagItem.Diploma] = 0;
        Bag.Items[(int)TBagItem.Pipe] = 0;
        Player.Practice = 1;
    }

    private static void PokojAt(string cmds, short room)
    {
        Session.Room = room;
        using (FeedStdin(cmds))
        {
            switch (room)
            {
                case 4: Rooms.Room4(); break;
                case 11: Rooms.Room11(); break;
                case 13: Rooms.Room13(); break;
                case 30: Rooms.Room30(); break;
                case 60: Rooms.Room60(); break;
                case 75: Rooms.Room75(); break;
                case 83: Rooms.Room83(); break;
                case 100: Rooms.Room100(); break;
                default: Rooms.PlainRooms(); break;
            }
        }
    }

    [Fact]
    public void Pokoj5Poster()
    {
        RoomSetup();
        Session.Room = 5;
        using (FeedStdin("EXIT|PATRZ PLAKAT|WYJSCIE"))
            Rooms.Room5();
        Assert.Equal(5, Session.Room);
    }

    [Fact]
    public void Pokoj1Poster()
    {
        RoomSetup();
        Session.Room = 1;
        using (FeedStdin("EXIT|PATRZ PLAKAT|WYJSCIE"))
            Rooms.Room1();
        Assert.Equal(1, Session.Room);
    }

    [Fact]
    public void Pokoj4Poster()
    {
        RoomSetup();
        PokojAt("EXIT|PATRZ AFISZ|WYJSCIE", 4);
        Assert.Equal(4, Session.Room);
    }

    [Fact]
    public void Pokoj13Loot()
    {
        RoomSetup();
        Bag.Items[(int)TBagItem.OldSword] = 0;
        Bag.Items[(int)TBagItem.SmallShield] = 0;
        Bag.Items[(int)TBagItem.Heart] = 0;
        bool sawSword = false, sawShield = false, sawHeart = false;
        for (int i = 1; i <= 80; i++)
        {
            Player.Energy = 30000; Player.Experience = 100;
            Player.Stats[(int)TStat.Strength] = 127;
            Player.Stats[(int)TStat.Dexterity] = 127;
            Combat.Fled = 0; Skills.Values[(int)TSkill.Fleeing] = 0;
            Skills.Values[(int)TSkill.FleeEnergy] = 0; Player.Mana = 0;
            Skills.Values[(int)TSkill.Parry] = 0;
            Combat.FireballCharges = 0; Combat.PoisonCharges = 0;
            Combat.PoisonRounds = 0; Skills.Values[(int)TSkill.Cooking] = 0;
            Skills.Values[(int)TSkill.Kick] = 0;
            Skills.Values[(int)TSkill.KickMinMana] = 0;
            Skills.Values[(int)TSkill.KickMaxEnergy] = 0;
            Gear.Values[(int)TGearItem.LuckRoll] = 0;
            World.Monsters[(int)TMonster.CageStrong] = 13;
            PokojAt("ZABIJ POTWOR|WYJSCIE", 13);
            if (Bag.Items[(int)TBagItem.OldSword] != 0)
                sawSword = true;
            if (Bag.Items[(int)TBagItem.SmallShield] != 0)
                sawShield = true;
            if (Bag.Items[(int)TBagItem.Heart] != 0)
                sawHeart = true;
            if (sawSword && sawShield && sawHeart)
                break;
        }
        Assert.True(sawSword);
        Assert.True(sawShield);
        Assert.True(sawHeart);
    }

    [Fact]
    public void Pokoj13Flee()
    {
        RoomSetup();
        World.Monsters[(int)TMonster.CageStrong] = 13;
        Player.Energy = 50; Skills.Values[(int)TSkill.FleeEnergy] = 100;
        Player.Mana = 20; Skills.Values[(int)TSkill.Fleeing] = 100;
        PokojAt("ZABIJ POTWOR|WYJSCIE", 13);
        Assert.Equal(13, World.Monsters[(int)TMonster.CageStrong]);
        Assert.Equal(0, Combat.Fled);
    }

    [Fact]
    public void Pokoj13Poster()
    {
        RoomSetup();
        World.Monsters[(int)TMonster.CageStrong] = 0;
        PokojAt("EXIT|MODE", 13);
        Assert.Equal(1000, Session.Room);
    }

    [Fact]
    public void PokojeSmallRooms()
    {
        short[] rooms = { 6, 8, 7, 10 };
        foreach (short room in rooms)
        {
            RoomSetup();
            PokojAt("EXIT|MODE", room);
            Assert.Equal(1000, Session.Room);
            RoomSetup();
            PokojAt("WYJSCIE", room);
            Assert.Equal(room, Session.Room);
        }
    }

    [Fact]
    public void Pokoj11Poster()
    {
        RoomSetup();
        PokojAt("EXIT|PATRZ PLAKAT|WYJSCIE", 11);
        Assert.Equal(11, Session.Room);
    }

    [Fact]
    public void Pokoj30Exits()
    {
        RoomSetup();
        PokojAt("EXIT|WYJSCIE", 30);
        Assert.Equal(30, Session.Room);
    }

    [Fact]
    public void Pokoj60Exits()
    {
        RoomSetup();
        PokojAt("EXIT|WYJSCIE", 60);
        Assert.Equal(60, Session.Room);
    }

    [Fact]
    public void Pokoj75Exits()
    {
        RoomSetup();
        PokojAt("EXIT|WYJSCIE", 75);
        Assert.Equal(75, Session.Room);
    }

    [Fact]
    public void Pokoj83Exits()
    {
        RoomSetup();
        PokojAt("EXIT|WYJSCIE", 83);
        Assert.Equal(83, Session.Room);
    }

    [Fact]
    public void Pokoj100QuestMaster()
    {
        RoomSetup();
        Player.Money = 10000; Bag.Permit = 0;
        PokojAt("EXIT|LISTA|ZABIJ QUEST-MASTER|KUP LATWY QUEST|WYJSCIE", 100);
        Assert.Equal(1, World.Quest);
        Assert.Equal(75, World.QuestKills);
        Assert.Equal(9800, Player.Money);
        Assert.Equal(-10, Bag.Permit);
    }

    [Fact]
    public void Pokoj100BlockedRoad()
    {
        RoomSetup();
        Bag.Permit = 0;
        PokojAt("ZACHOD|WYJSCIE", 100);
        Assert.Equal(100, Session.Room);
    }

    [Fact]
    public void Pokoj100OpenRoad()
    {
        RoomSetup();
        Bag.Permit = -20;
        PokojAt("EXIT|ZACHOD", 100);
        Assert.Equal(101, Session.Room);
    }

    [Fact]
    public void Pokoj100QuestPrices()
    {
        RoomSetup();
        Player.Money = 10000;
        PokojAt("KUP PRZECIETNY QUEST|KUP TRUDNY QUEST|WYJSCIE", 100);
        Assert.Equal(3, World.Quest);
        Assert.Equal(200, World.QuestKills);
        Assert.Equal(9850, Player.Money);
    }

    [Fact]
    public void Pokoj100SellEasy()
    {
        RoomSetup();
        World.Quest = 1; World.QuestKills = 0; Bag.Permit = 0;
        Player.Experience = 100;
        PokojAt("SPRZEDAJ QUEST|WYJSCIE", 100);
        Assert.Equal(0, World.Quest);
        Assert.Equal(0, World.QuestKills);
        Assert.Equal(-10, Bag.Permit);
        Assert.Equal(200, Player.Experience);
        Assert.Equal(1, Bag.Count);
    }

    [Fact]
    public void Pokoj100SellMedium()
    {
        RoomSetup();
        World.Quest = 2; World.QuestKills = 0;
        Bag.Items[(int)TBagItem.Diploma] = -10; Player.MaxEnergy = 50;
        Bag.Permit = 0; Player.Experience = 100;
        PokojAt("SPRZEDAJ QUEST|WYJSCIE", 100);
        Assert.Equal(0, World.Quest);
        Assert.Equal(0, Bag.Items[(int)TBagItem.Diploma]);
        Assert.Equal(45, Player.MaxEnergy);
        Assert.Equal(-10, Bag.Permit);
        Assert.Equal(350, Player.Experience);
    }

    [Fact]
    public void Pokoj100SellHard()
    {
        RoomSetup();
        World.Quest = 3; World.QuestKills = 0;
        Bag.Items[(int)TBagItem.Pipe] = -10; Player.Practice = 1;
        Bag.Permit = 0; Player.Experience = 100; Bag.Count = 0;
        Player.Stats[(int)TStat.Wisdom] = 5;
        PokojAt("SPRZEDAJ QUEST|WYJSCIE", 100);
        Assert.Equal(0, World.Quest);
        Assert.Equal(0, Bag.Items[(int)TBagItem.Pipe]);
        Assert.Equal(0, Player.Practice);
        Assert.Equal(-10, Bag.Permit);
        Assert.Equal(525, Player.Experience);
        Assert.Equal(-1, Bag.Count);
        Assert.Equal(4, Player.Stats[(int)TStat.Wisdom]);
    }
}
