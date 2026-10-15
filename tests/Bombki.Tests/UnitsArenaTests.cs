using static Game;

namespace Bombki.Tests;

// Port of the describe/miniarena/fight/porownanie/heart-drop unit tests
// from test_przedm.pas. Command-driven procedures use Crt.TestInput.
public sealed class UnitsArenaTests
{
    private static TestStdin FeedStdin(string lines) => TestStdin.Feed(lines);

    [Fact]
    public void ScenaMessages()
    {
        Session.Room = 1;
        World.Monsters[(int)TMonster.Guitarist] = 1;
        World.Monsters[(int)TMonster.Drummer] = 1;
        World.Monsters[(int)TMonster.Keyboardist] = 1;
        World.Monsters[(int)TMonster.Liroy] = 1;
        DescribeStage();
        Assert.Equal(1, Session.Room);
    }

    [Fact]
    public void TlumMessages()
    {
        Session.Room = 1;
        World.Monsters[(int)TMonster.Kid] = 1;
        World.Monsters[(int)TMonster.Lunatic] = 1;
        World.Monsters[(int)TMonster.Listener] = 1;
        World.Monsters[(int)TMonster.Fan] = 1;
        World.Monsters[(int)TMonster.Man] = 1;
        World.Monsters[(int)TMonster.Policeman] = 1;
        World.Monsters[(int)TMonster.Bouncer] = 1;
        World.Monsters[(int)TMonster.Gorilla] = 1;
        World.Monsters[(int)TMonster.OldMan] = 1;
        World.Monsters[(int)TMonster.Reporter] = 1;
        DescribeCrowd();
        Assert.Equal(1, Session.Room);
    }

    [Fact]
    public void KtoMessages()
    {
        Session.Room = 1;
        World.Monsters[(int)TMonster.Woodworm] = 1;
        World.Monsters[(int)TMonster.Fly] = 1;
        World.Monsters[(int)TMonster.Bacterium] = 1;
        World.Monsters[(int)TMonster.Snail] = 1;
        World.Monsters[(int)TMonster.Beetle] = 1;
        World.Monsters[(int)TMonster.Cockroach] = 1;
        World.Monsters[(int)TMonster.Ant] = 1;
        World.Monsters[(int)TMonster.Spider] = 1;
        World.Monsters[(int)TMonster.Boar] = 1;
        World.Monsters[(int)TMonster.Rat] = 1;
        World.Monsters[(int)TMonster.Fox] = 1;
        World.Monsters[(int)TMonster.Partridge] = 1;
        World.Monsters[(int)TMonster.Hare] = 1;
        World.Monsters[(int)TMonster.Mongrel] = 1;
        World.Monsters[(int)TMonster.Eagle] = 1;
        World.Monsters[(int)TMonster.Deer] = 1;
        World.Monsters[(int)TMonster.Elephant] = 1;
        World.Monsters[(int)TMonster.Lion] = 1;
        World.Monsters[(int)TMonster.Giraffe] = 1;
        World.Monsters[(int)TMonster.Camel] = 1;
        World.Monsters[(int)TMonster.Ostrich] = 1;
        World.Monsters[(int)TMonster.Boa] = 1;
        World.Monsters[(int)TMonster.Wolf] = 1;
        World.Monsters[(int)TMonster.Bison] = 1;
        World.Monsters[(int)TMonster.Panther] = 1;
        World.Monsters[(int)TMonster.Gladiator] = 1;
        World.Monsters[(int)TMonster.Warrior] = 1;
        World.Monsters[(int)TMonster.Tamer] = 1;
        DescribeBeasts();
        Assert.Equal(1, Session.Room);
    }

    [Fact]
    public void UlSklepMessages()
    {
        Session.Room = 1;
        World.Monsters[(int)TMonster.Dachshund] = 1;
        World.Monsters[(int)TMonster.Shepherd] = 1;
        World.Monsters[(int)TMonster.Pooch] = 1;
        World.Monsters[(int)TMonster.Spaniel] = 1;
        World.Monsters[(int)TMonster.Poodle] = 1;
        World.Monsters[(int)TMonster.Cabman] = 1;
        World.Monsters[(int)TMonster.Vendor] = 1;
        World.Monsters[(int)TMonster.Sweeper] = 1;
        World.Monsters[(int)TMonster.Drunkard] = 1;
        World.Monsters[(int)TMonster.SickMan] = 1;
        DescribeStreet();
        Assert.Equal(1, Session.Room);
    }

    [Fact]
    public void BluszczMessages()
    {
        Session.Room = 1;
        World.Monsters[(int)TMonster.Sorrel] = 1;
        World.Monsters[(int)TMonster.Daisy] = 1;
        World.Monsters[(int)TMonster.Clover] = 1;
        World.Monsters[(int)TMonster.SowThistle] = 1;
        World.Monsters[(int)TMonster.Dandelion] = 1;
        World.Monsters[(int)TMonster.Rose] = 1;
        World.Monsters[(int)TMonster.Thistle] = 1;
        World.Monsters[(int)TMonster.Blackberry] = 1;
        World.Monsters[(int)TMonster.Gooseberry] = 1;
        World.Monsters[(int)TMonster.Raspberry] = 1;
        World.Monsters[(int)TMonster.Grass] = 1;
        World.Monsters[(int)TMonster.Duncan] = 1;
        DescribePlants();
        Assert.Equal(1, Session.Room);
    }

    [Fact]
    public void PierdolyTlo()
    {
        using (FeedStdin("1"))
        {
            Command = "ZMIEN TLO";
            DisplayPrefs();
        }
    }

    [Fact]
    public void Porownanie()
    {
        Player.Mana = 10;
        using (FeedStdin("DZIK"))
            CompareFoe();
    }

    private static void MiniarenaSetup()
    {
        World.Monsters[(int)TMonster.Bacterium] = 33;
        World.Monsters[(int)TMonster.Snail] = 33;
        World.Monsters[(int)TMonster.Woodworm] = 33;
        World.Monsters[(int)TMonster.Fly] = 33;
        World.Monsters[(int)TMonster.Beetle] = 33;
        World.Monsters[(int)TMonster.Cockroach] = 33;
        World.Monsters[(int)TMonster.Ant] = 33;
        World.Monsters[(int)TMonster.Spider] = 33;
        World.Monsters[(int)TMonster.Boar] = 33;
        World.Monsters[(int)TMonster.Mongrel] = 33;
        World.Monsters[(int)TMonster.Rat] = 33;
        World.Monsters[(int)TMonster.Fox] = 33;
        World.Monsters[(int)TMonster.Partridge] = 33;
        World.Monsters[(int)TMonster.Hare] = 33;
        World.Monsters[(int)TMonster.Eagle] = 33;
        World.Monsters[(int)TMonster.Deer] = 33;
        World.Monsters[(int)TMonster.Elephant] = 33;
        World.Monsters[(int)TMonster.Lion] = 33;
        World.Monsters[(int)TMonster.Giraffe] = 33;
        World.Monsters[(int)TMonster.Wolf] = 33;
        World.Monsters[(int)TMonster.Camel] = 33;
        World.Monsters[(int)TMonster.Ostrich] = 33;
        World.Monsters[(int)TMonster.Boa] = 33;
        World.Monsters[(int)TMonster.Bison] = 33;
        World.Monsters[(int)TMonster.Panther] = 33;
        World.Monsters[(int)TMonster.Gladiator] = 33;
        World.Monsters[(int)TMonster.Warrior] = 33;
        World.Monsters[(int)TMonster.Tamer] = 33;
        Session.Room = 33;
        Player.Stats[(int)TStat.Strength] = 127;
        Player.Stats[(int)TStat.Dexterity] = 127;
        Player.Energy = 30000; Combat.Fled = 0;
    }

    [Fact]
    public void MiniarenaSlabo()
    {
        MiniarenaSetup();
        using (FeedStdin("ZABIJ BAKTERIA|ZABIJ SLIMAK|ZABIJ KORNIK|ZABIJ MUCHA|"
            + "ZABIJ ZUK|ZABIJ KARALUCH|ZABIJ MROWKA|ZABIJ PAJAK|MODE"))
            ArenaMaze();
    }

    [Fact]
    public void MiniarenaMniejslabo()
    {
        MiniarenaSetup();
        using (FeedStdin("ZABIJ DZIK|ZABIJ WILCZUR|ZABIJ SZCZUR|ZABIJ LIS|"
            + "ZABIJ KUROPATWA|ZABIJ ZAJAC|ZABIJ ORZEL|ZABIJ SARNA|MODE"))
            ArenaMaze();
    }

    [Fact]
    public void MiniarenaSrednio()
    {
        MiniarenaSetup();
        using (FeedStdin("ZABIJ SLON|ZABIJ LEW|ZABIJ ZYRAFA|ZABIJ WILK|"
            + "ZABIJ WIELBLAD|ZABIJ STRUS|ZABIJ BOA|ZABIJ BIZON|"
            + "ZABIJ PANTERA|MODE"))
            ArenaMaze();
    }

    [Fact]
    public void MiniarenaTrudno()
    {
        MiniarenaSetup();
        using (FeedStdin("ZABIJ GLADIATOR|ZABIJ WOJOWNIK|ZABIJ TRENER|MODE"))
            ArenaMaze();
    }

    private static void MiniarenaAt(string cmd, short room)
    {
        Session.Room = room;
        using (FeedStdin(cmd))
            ArenaMaze();
    }

    [Fact]
    public void MiniarenaExitNav()
    {
        MiniarenaAt("EXIT|MODE", 33);
        MiniarenaAt("EXIT|MODE", 34);
        MiniarenaAt("EXIT|MODE", 35);
        MiniarenaAt("EXIT|MODE", 40);
        MiniarenaAt("EXIT|MODE", 41);
        MiniarenaAt("EXIT|MODE", 45);
        MiniarenaAt("EXIT|MODE", 46);
        MiniarenaAt("EXIT|MODE", 47);
        MiniarenaAt("EXIT|MODE", 56);
        MiniarenaAt("EXIT|MODE", 57);
        MiniarenaAt("POLODNIE", 33);
        MiniarenaAt("POLNOC", 33);
        MiniarenaAt("WSCHOD", 33);
        MiniarenaAt("ZACHOD", 33);
    }

    private static void FightSetup()
    {
        Session.Room = 100;
        World.Monsters[(int)TMonster.Drummer] = 100;
        World.Monsters[(int)TMonster.Guitarist] = 100;
        World.Monsters[(int)TMonster.Keyboardist] = 100;
        World.Monsters[(int)TMonster.Liroy] = 100;
        World.Monsters[(int)TMonster.Duncan] = 100;
        World.Monsters[(int)TMonster.Sorrel] = 100;
        World.Monsters[(int)TMonster.Daisy] = 100;
        World.Monsters[(int)TMonster.Clover] = 100;
        World.Monsters[(int)TMonster.SowThistle] = 100;
        World.Monsters[(int)TMonster.Dandelion] = 100;
        World.Monsters[(int)TMonster.Rose] = 100;
        World.Monsters[(int)TMonster.Thistle] = 100;
        World.Monsters[(int)TMonster.Raspberry] = 100;
        World.Monsters[(int)TMonster.Gooseberry] = 100;
        World.Monsters[(int)TMonster.Blackberry] = 100;
        World.Monsters[(int)TMonster.Grass] = 100;
        Player.Stats[(int)TStat.Strength] = 127;
        Player.Stats[(int)TStat.Dexterity] = 127;
        Player.Energy = 30000; Combat.Fled = 0;
        World.DuncanQuest = 0; Player.Money = 10000; Bag.Permit = 100;
        Bag.Items[(int)TBagItem.Backpack] = 100;
        World.Quest = 0; Player.Experience = 0;
    }

    [Fact]
    public void Fightscena()
    {
        FightSetup();
        Command = "ZABIJ PERKUSISTA"; StageFights();
        Command = "ZABIJ GITARZYSTA"; StageFights();
        Command = "ZABIJ LIROY"; StageFights();
        Command = "ZABIJ ORGANISTA"; StageFights();
    }

    [Fact]
    public void FightbluszczZabij()
    {
        FightSetup();
        Command = "ZABIJ DUNCAN"; PlantFights();
        Command = "ZABIJ SZCZAW"; PlantFights();
        Command = "ZABIJ STOKROTKA"; PlantFights();
        Command = "ZABIJ KONICZYNKA"; PlantFights();
        Command = "ZABIJ MLECZ"; PlantFights();
        Command = "ZABIJ DMUCHAWIEC"; PlantFights();
        Command = "ZABIJ ROZA"; PlantFights();
        Command = "ZABIJ OSET"; PlantFights();
        Command = "ZABIJ MALINA"; PlantFights();
        Command = "ZABIJ AGREST"; PlantFights();
        Command = "ZABIJ JEZYNA"; PlantFights();
        Command = "ZABIJ TRAWA"; PlantFights();
    }

    [Fact]
    public void FightbluszczShop()
    {
        FightSetup();
        Command = "SECRET LISTA"; PlantFights();
        Command = "KUP DOKUMENT"; PlantFights();
        Command = "KUP PLECAK"; PlantFights();
    }

    [Fact]
    public void FightbluszczDuncan()
    {
        FightSetup();
        World.DuncanQuest = 0;
        Command = "ROZMAWIAJ DUNCAN"; PlantFights();
        World.DuncanQuest = -125;
        Command = "ROZMAWIAJ DUNCAN"; PlantFights();
    }

    private static void PorownanieAsk(string monster,
        int poziom, int sil, int zre, int par, int kop)
    {
        using (FeedStdin(monster))
        {
            Player.Mana = 10; Player.Level = (sbyte)poziom;
            Player.Stats[(int)TStat.Strength] = (short)sil;
            Player.Stats[(int)TStat.Dexterity] = (short)zre;
            Skills.Values[(int)TSkill.Parry] = (short)par;
            Skills.Values[(int)TSkill.Kick] = (short)kop;
            CompareFoe();
        }
    }

    [Fact]
    public void PorownanieBeasts()
    {
        PorownanieAsk("DZIK", 0, 0, 0, 0, 0);
        PorownanieAsk("KUROPATWA", 0, 16, 0, 0, 0);
        PorownanieAsk("SARNA", 0, 19, 0, 0, 0);
        PorownanieAsk("DZIADEK", 0, 30, 0, 0, 0);
        Assert.Equal(30, Player.Stats[(int)TStat.Strength]);
    }

    [Fact]
    public void PorownanieBigBeasts()
    {
        PorownanieAsk("SLON", 0, 0, 0, 0, 0);
        PorownanieAsk("LEW", 0, 16, 0, 0, 0);
        PorownanieAsk("ZYRAFA", 0, 21, 0, 0, 0);
        PorownanieAsk("REPORTER", 0, 23, 0, 0, 0);
    }

    [Fact]
    public void PorownanieFighters()
    {
        PorownanieAsk("GLADIATOR", 0, 0, 0, 0, 0);
        PorownanieAsk("WOJOWNIK", 0, 16, 0, 0, 0);
        PorownanieAsk("TRENER", 0, 20, 0, 0, 0);
        PorownanieAsk("POLICJANT", 0, 24, 0, 0, 0);
        PorownanieAsk("GORYL", 0, 27, 0, 0, 0);
        PorownanieAsk("OCHRONIARZ", 0, 30, 0, 0, 0);
    }

    [Fact]
    public void PorownanieDrunks()
    {
        PorownanieAsk("MINI-BARMAN", 0, 0, 0, 0, 0);
        PorownanieAsk("GRUBAS", 0, 25, 0, 0, 0);
        PorownanieAsk("D.J", 0, 27, 0, 0, 0);
        PorownanieAsk("GRUBAS", 0, 29, 0, 0, 0);
        PorownanieAsk("D.J", 0, 35, 0, 0, 0);
    }

    [Fact]
    public void PorownanieMusicians()
    {
        PorownanieAsk("TAKSOWKARZ", 0, 0, 0, 0, 0);
        PorownanieAsk("GITARZYSTA", 0, 23, 0, 0, 0);
        PorownanieAsk("PERKUSISTA", 0, 26, 0, 0, 0);
        PorownanieAsk("ORGANISTA", 0, 29, 0, 0, 0);
        PorownanieAsk("ZEBRAK", 0, 30, 0, 0, 0);
    }

    [Fact]
    public void PorownanieBluszcz()
    {
        PorownanieAsk("PEDAL", 0, 0, 0, 0, 0);
        PorownanieAsk("PARA", 0, 20, 0, 0, 0);
        PorownanieAsk("MACIEK", 0, 26, 0, 0, 0);
        PorownanieAsk("ROZA", 0, 30, 0, 0, 0);
        PorownanieAsk("MALINA", 0, 36, 0, 0, 0);
        PorownanieAsk("TRAWA", 0, 0, 9, 0, 0);
        PorownanieAsk("AGREST", 0, 0, 11, 0, 0);
        PorownanieAsk("JEZYNA", 0, 0, 13, 0, 0);
        PorownanieAsk("TRAWA", 0, 0, 15, 0, 0);
        PorownanieAsk("AGREST", 0, 0, 17, 0, 0);
        PorownanieAsk("JEZYNA", 0, 0, 19, 0, 0);
        PorownanieAsk("TRAWA", 0, 0, 21, 0, 0);
    }

    [Fact]
    public void PorownanieLiroyAndDogs()
    {
        PorownanieAsk("LIROY", 0, 0, 0, 0, 0);
        PorownanieAsk("LIROY", 0, 28, 0, 0, 0);
        PorownanieAsk("LIROY", 0, 33, 0, 0, 0);
        PorownanieAsk("LIROY", 0, 44, 0, 0, 0);
        PorownanieAsk("LIROY", 0, 48, 0, 0, 0);
        PorownanieAsk("POKRZYWA", 0, 0, 0, 0, 0);
        PorownanieAsk("POKRZYWA", 0, 41, 0, 0, 0);
        PorownanieAsk("SPANIEL", 0, 0, 0, 0, 0);
        PorownanieAsk("OWCZAREK", 0, 0, 0, 0, 0);
        PorownanieAsk("PIESEK", 0, 0, 0, 0, 0);
        PorownanieAsk("JAMNIK", 0, 0, 0, 0, 0);
        PorownanieAsk("PUDEL", 0, 0, 0, 0, 0);
    }

    [Fact]
    public void PorownanieBonusesAndBakteria()
    {
        PorownanieAsk("DZIK", 0, 10, 0, 51, 11);
        PorownanieAsk("DZIK", 0, 10, 0, 76, 71);
        Player.Mana = 10;
        using (FeedStdin("BAKTERIA"))
            CompareFoe();
        Assert.Equal(15, Player.Mana);
    }

    private static void LootSetup()
    {
        Bag.Items[(int)TBagItem.Heart] = 0;
        Player.Energy = 30000; Player.Stats[(int)TStat.Strength] = 127;
        Player.Stats[(int)TStat.Dexterity] = 127; Combat.Fled = 0;
        Skills.Values[(int)TSkill.Fleeing] = 0;
        Skills.Values[(int)TSkill.FleeEnergy] = 0; Player.Mana = 0;
        Combat.FireballCharges = 0;
        Combat.PoisonCharges = 0; Combat.PoisonRounds = 0;
        Skills.Values[(int)TSkill.Kick] = 0;
        Skills.Values[(int)TSkill.KickMinMana] = 0;
        Skills.Values[(int)TSkill.KickMaxEnergy] = 0;
        Gear.Values[(int)TGearItem.Armor] = 0;
        Gear.Values[(int)TGearItem.WornCount] = 0;
        Gear.Values[(int)TGearItem.LuckRoll] = 0;
        Bag.Count = 0;
    }

    [Fact]
    public void EncounterHeartVeasy()
    {
        LootSetup();
        for (int i = 1; i <= 160; i++)
            if (Bag.Items[(int)TBagItem.Heart] == 0)
                Fight(TFoe.VeryEasyEnemy);
        Assert.Equal(-10, Bag.Items[(int)TBagItem.Heart]);
    }

    [Fact]
    public void EncounterHeartEasy()
    {
        LootSetup();
        for (int i = 1; i <= 160; i++)
            if (Bag.Items[(int)TBagItem.Heart] == 0)
                Fight(TFoe.EasyEnemy);
        Assert.Equal(-10, Bag.Items[(int)TBagItem.Heart]);
    }

    [Fact]
    public void EncounterHeartNeasy()
    {
        LootSetup();
        for (int i = 1; i <= 160; i++)
            if (Bag.Items[(int)TBagItem.Heart] == 0)
                Fight(TFoe.NotEasyEnemy);
        Assert.Equal(-10, Bag.Items[(int)TBagItem.Heart]);
    }

    [Fact]
    public void EncounterHeartTrudno()
    {
        LootSetup();
        for (int i = 1; i <= 110; i++)
            if (Bag.Items[(int)TBagItem.Heart] == 0)
                Fight(TFoe.HardEnemy);
        Assert.Equal(-10, Bag.Items[(int)TBagItem.Heart]);
    }

    [Fact]
    public void EncounterHeartBtrudno()
    {
        LootSetup();
        for (int i = 1; i <= 150; i++)
            if (Bag.Items[(int)TBagItem.Heart] == 0)
                Fight(TFoe.VeryHardEnemy);
        Assert.Equal(-10, Bag.Items[(int)TBagItem.Heart]);
    }

    [Fact]
    public void WalkaPotrawkiLearn()
    {
        bool learned = false;
        for (int i = 1; i <= 800; i++)
        {
            Player.Energy = 100; Player.Stats[(int)TStat.Strength] = 127;
            Player.Stats[(int)TStat.Dexterity] = 127; Player.MaxEnergy = 0;
            Combat.EnemyEnergy = 0;
            Combat.EnemyStrength = 1;
            Combat.EnemyDexterity = 1;
            Skills.Values[(int)TSkill.Parry] = 0;
            Skills.Values[(int)TSkill.Kick] = 0;
            Skills.Values[(int)TSkill.KickMinMana] = 0;
            Skills.Values[(int)TSkill.KickMaxEnergy] = 0;
            Player.Mana = 0;
            Combat.FireballCharges = 0; Combat.PoisonCharges = 0;
            Combat.PoisonRounds = 0;
            Skills.Values[(int)TSkill.Fleeing] = 0;
            Skills.Values[(int)TSkill.FleeEnergy] = 0;
            Combat.Fled = 0;
            Player.Experience = 0;
            Gear.Values[(int)TGearItem.LuckRoll] = 0;
            Gear.Values[(int)TGearItem.Armor] = 0;
            Gear.Values[(int)TGearItem.WornCount] = 0;
            Player.Energy = 100; Skills.Values[(int)TSkill.Cooking] = 1;
            Bag.Items[(int)TBagItem.Bigos] = 0;
            DoFight();
            if (Skills.Values[(int)TSkill.Cooking] > 1)
                learned = true;
        }
        Assert.True(learned);
    }

    [Fact]
    public void WalkaParryLearn()
    {
        for (int i = 1; i <= 60; i++)
        {
            Player.Energy = 100; Player.Stats[(int)TStat.Strength] = 127;
            Player.Stats[(int)TStat.Dexterity] = 127; Player.MaxEnergy = 0;
            Combat.EnemyEnergy = 3000;
            Combat.EnemyStrength = 5;
            Combat.EnemyDexterity = 1;
            Skills.Values[(int)TSkill.Parry] = 99;
            Skills.Values[(int)TSkill.Kick] = 0;
            Skills.Values[(int)TSkill.KickMinMana] = 0;
            Skills.Values[(int)TSkill.KickMaxEnergy] = 0;
            Player.Mana = 0;
            Combat.FireballCharges = 0; Combat.PoisonCharges = 0;
            Combat.PoisonRounds = 0;
            Skills.Values[(int)TSkill.Fleeing] = 0;
            Skills.Values[(int)TSkill.FleeEnergy] = 0;
            Combat.Fled = 0;
            Player.Experience = 0;
            Gear.Values[(int)TGearItem.LuckRoll] = 0;
            Gear.Values[(int)TGearItem.Armor] = 0;
            Gear.Values[(int)TGearItem.WornCount] = 0;
            Player.Energy = 30000;
            DoFight();
            if (Skills.Values[(int)TSkill.Parry] > 99)
                break;
        }
        Assert.True(Skills.Values[(int)TSkill.Parry] > 99);
    }
}
