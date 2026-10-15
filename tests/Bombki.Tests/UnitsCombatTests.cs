using static Game;

namespace Bombki.Tests;

// Port of the WALKA combat unit tests from test_przedm.pas: dead-enemy
// cases are exact (one deterministic iteration), live-enemy cases are
// ranges over the Random damage sequence, specials are boolean.
public sealed class UnitsCombatTests
{
    private static void WalkaSetup(int maxE, int wrogE, int sil, int wrogSil,
        int zre, int wrogZre, int par, int kop)
    {
        Player.Energy = 100;
        Skills.Values[(int)TSkill.Parry] = (short)par;
        Skills.Values[(int)TSkill.Kick] = (short)kop;
        Skills.Values[(int)TSkill.KickMinMana] = 0;
        Skills.Values[(int)TSkill.KickMaxEnergy] = 0;
        Player.Mana = 0;
        Combat.FireballCharges = 0;
        Combat.PoisonCharges = 0;
        Combat.PoisonRounds = 0;
        Skills.Values[(int)TSkill.Fleeing] = 0;
        Skills.Values[(int)TSkill.FleeEnergy] = 0;
        Combat.Fled = 0;
        Player.Experience = 0;
        Gear.Values[(int)TGearItem.LuckRoll] = 0;
        Gear.Values[(int)TGearItem.Armor] = 0;
        Gear.Values[(int)TGearItem.WornCount] = 0;
        Player.MaxEnergy = (short)maxE;
        Combat.EnemyEnergy = (short)wrogE;
        Player.Stats[(int)TStat.Strength] = (short)sil;
        Combat.EnemyStrength = (short)wrogSil;
        Player.Stats[(int)TStat.Dexterity] = (short)zre;
        Combat.EnemyDexterity = (short)wrogZre;
    }

    private static void RunWalka(int maxE, int wrogE, int sil, int wrogSil,
        int zre, int wrogZre, int par, int kop, int expectedKunszt)
    {
        WalkaSetup(maxE, wrogE, sil, wrogSil, zre, wrogZre, par, kop);
        DoFight();
        Assert.Equal(expectedKunszt, Player.Experience);
    }

    private static void RunWalkaRange(int maxE, int wrogE, int sil, int wrogSil,
        int zre, int wrogZre, int par, int kop, int kunsztLo, int kunsztHi)
    {
        WalkaSetup(maxE, wrogE, sil, wrogSil, zre, wrogZre, par, kop);
        DoFight();
        Assert.InRange(Player.Experience, kunsztLo, kunsztHi);
    }

    [Fact] public void WalkaEqualStats() =>
        RunWalka(0, 0, 1, 1, 10, 10, 0, 0, 34);
    [Fact] public void WalkaMaxePlusOne() =>
        RunWalka(1, 0, 1, 1, 10, 10, 0, 0, 33);
    [Fact] public void WalkaMaxePlusThreeOverlap() =>
        RunWalka(3, 0, 1, 1, 10, 10, 0, 0, 42);
    [Fact] public void WalkaMaxePlusFive() =>
        RunWalka(5, 0, 1, 1, 10, 10, 0, 0, 32);
    [Fact] public void WalkaMaxePlusEleven() =>
        RunWalka(11, 0, 1, 1, 10, 10, 0, 0, 30);
    [Fact] public void WalkaMaxePlusTwenty() =>
        RunWalka(20, 0, 1, 1, 10, 10, 0, 0, 27);
    [Fact] public void WalkaMaxePlusTwentyNine() =>
        RunWalka(29, 0, 1, 1, 10, 10, 0, 0, 24);
    [Fact] public void WalkaMaxePlusThirtyOne() =>
        RunWalka(31, 0, 1, 1, 10, 10, 0, 0, 23);
    [Fact] public void WalkaMaxeBandAdjustment() =>
        RunWalka(76, 0, 1, 1, 10, 10, 0, 0, 21);
    [Fact] public void WalkaMaxeHugeAdjustment() =>
        RunWalka(116, 0, 1, 1, 10, 10, 0, 0, 18);
    [Fact] public void WalkaMaxeMinusTwo() =>
        RunWalkaRange(0, 2, 61, 1, 10, 10, 0, 0, 24, 26);
    [Fact] public void WalkaMaxeMinusTen() =>
        RunWalkaRange(0, 10, 61, 1, 10, 10, 0, 0, 28, 31);
    [Fact] public void WalkaMaxeMinusTwentyOne() =>
        RunWalkaRange(0, 21, 61, 1, 10, 10, 0, 0, 33, 36);
    [Fact] public void WalkaSilPlusOne() =>
        RunWalka(0, 0, 11, 10, 10, 10, 0, 0, 33);
    [Fact] public void WalkaSilPlusTen() =>
        RunWalka(0, 0, 20, 10, 10, 10, 0, 0, 24);
    [Fact] public void WalkaSilMinusOne() =>
        RunWalka(0, 0, 9, 10, 10, 10, 0, 0, 35);
    [Fact] public void WalkaSilMinusTen() =>
        RunWalka(0, 0, 1, 11, 10, 10, 0, 0, 44);
    [Fact] public void WalkaParAdjustments() =>
        RunWalka(0, 0, 1, 1, 10, 10, 96, 0, 29);
    [Fact] public void WalkaKopAdjustments() =>
        RunWalka(0, 0, 1, 1, 10, 10, 0, 96, 27);
    [Fact] public void WalkaZrePlusOne() =>
        RunWalkaRange(0, 0, 1, 1, 11, 10, 0, 0, 32, 33);
    [Fact] public void WalkaZrePlusTwelve() =>
        RunWalkaRange(0, 0, 1, 1, 22, 10, 0, 0, 27, 28);
    [Fact] public void WalkaZreMinusOne() =>
        RunWalkaRange(0, 0, 1, 1, 9, 10, 0, 0, 34, 35);
    [Fact] public void WalkaZreMinusTwelve() =>
        RunWalkaRange(0, 0, 1, 1, 1, 13, 0, 0, 39, 40);

    [Fact]
    public void WalkaPoisoned()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 0);
        Combat.PoisonRounds = 10;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Energy > 0);
    }

    [Fact]
    public void WalkaFireball()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 0);
        Combat.FireballCharges = 1;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Combat.FireballCharges == 0 || Combat.FireballCharges == 1);
    }

    [Fact]
    public void WalkaPoison()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 0);
        Combat.PoisonCharges = 1;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Combat.PoisonCharges == 0 || Combat.PoisonCharges == 1);
    }

    [Fact]
    public void WalkaParry()
    {
        WalkaSetup(0, 0, 1, 20, 10, 10, 139, 0);
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Energy > 0);
    }

    [Fact]
    public void WalkaParryLight()
    {
        WalkaSetup(0, 10, 3, 3, 10, 10, 139, 0);
        Player.Energy = 10000;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Energy > 0);
    }

    [Fact]
    public void WalkaSpecials()
    {
        WalkaSetup(0, 20, 3, 20, 10, 10, 139, 0);
        Player.Energy = 10000;
        Combat.FireballCharges = 1;
        Combat.PoisonCharges = 1;
        Combat.PoisonRounds = 10;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Energy > 0);
    }

    [Fact]
    public void WalkaSuperKopHit()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 100);
        Player.Mana = 100; Skills.Values[(int)TSkill.KickMaxEnergy] = 100;
        Player.Energy = 50;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Mana < 100);
    }

    [Fact]
    public void WalkaSuperKopHitAgain()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 100);
        Player.Mana = 100; Skills.Values[(int)TSkill.KickMaxEnergy] = 100;
        Player.Energy = 50;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Mana < 100);
    }

    [Fact]
    public void WalkaSuperKopChybia()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 11);
        Player.Mana = 100; Skills.Values[(int)TSkill.KickMaxEnergy] = 100;
        Player.Energy = 50;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Mana < 100);
    }

    [Fact]
    public void WalkaFlee()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 0);
        Skills.Values[(int)TSkill.Fleeing] = 100;
        Skills.Values[(int)TSkill.FleeEnergy] = 100;
        Player.Energy = 50; Player.Mana = 100;
        DoFight();
        Assert.True(Combat.Fled == 1);
    }

    [Fact]
    public void WalkaFleeChybia()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 0);
        Skills.Values[(int)TSkill.Fleeing] = 1;
        Skills.Values[(int)TSkill.FleeEnergy] = 100;
        Player.Energy = 50; Player.Mana = 100;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
    }

    [Fact]
    public void WalkaPotrawki()
    {
        WalkaSetup(0, 0, 1, 1, 10, 10, 0, 0);
        Skills.Values[(int)TSkill.Cooking] = 99;
        Bag.Items[(int)TBagItem.Bigos] = 50; Bag.Count = 0;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Bag.Count == 0 || Bag.Count == 1);
    }

    [Fact]
    public void WalkaDeath()
    {
        WalkaSetup(0, 0, 1, 61, 10, 10, 0, 0);
        Player.Energy = 1;
        DoFight();
        Assert.True(Player.Energy < 1 || Combat.EnemyEnergy < 1);
    }

    [Fact]
    public void WalkaFukroll()
    {
        WalkaSetup(0, 10, 2, 1, 10, 10, 0, 0);
        Gear.Values[(int)TGearItem.LuckRoll] = 1;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
    }

    private static void EncounterSetup()
    {
        WalkaSetup(127, 0, 127, 1, 127, 1, 0, 0);
        Bag.Items[(int)TBagItem.SmallShield] = 0;
        Skills.Values[(int)TSkill.Cooking] = 0;
        Combat.FireballCharges = 0; Combat.PoisonCharges = 0;
        Combat.PoisonRounds = 0; Bag.Items[(int)TBagItem.Heart] = 0;
    }

    [Fact]
    public void Encounters()
    {
        EncounterSetup();
        Command = "ZABIJ MROWKA";
        Fight(TFoe.WeakEnemy);
        Assert.True(Combat.EnemyEnergy < 1);
        EncounterSetup();
        FightDog();
        Assert.True(Combat.EnemyEnergy < 1);
        EncounterSetup();
        Fight(TFoe.WeakishEnemy);
        Assert.True(Combat.EnemyEnergy < 1);
        EncounterSetup();
        Fight(TFoe.MediumEnemy);
        Assert.True(Combat.EnemyEnergy < 1);
        EncounterSetup();
        Fight(TFoe.HardEnemy);
        Assert.True(Combat.EnemyEnergy < 1);
        EncounterSetup();
        Fight(TFoe.VeryEasyEnemy);
        Assert.True(Combat.EnemyEnergy < 1 && Player.Energy > 0);
        EncounterSetup();
        Fight(TFoe.EasyEnemy);
        Assert.True(Combat.EnemyEnergy < 1 && Player.Energy > 0);
        EncounterSetup();
        Fight(TFoe.NotEasyEnemy);
        Assert.True(Combat.EnemyEnergy < 1 && Player.Energy > 0);
        EncounterSetup();
        Fight(TFoe.VeryHardEnemy);
        Assert.True(Combat.EnemyEnergy < 1);
    }

    [Fact]
    public void WalkaDodge()
    {
        WalkaSetup(0, 30, 3, 1, 0, 2, 0, 0);
        Player.Energy = 100;
        DoFight();
        Assert.True(Combat.EnemyEnergy < 1);
        Assert.True(Player.Energy > 0);
    }
}
