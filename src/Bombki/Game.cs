// Literal port of src-pas/refactored/Game.pas (interface types/state plus the
// implementation procedures). Member names stay byte-close to the
// Pascal originals; only language-forced adaptations differ:
//   Integer -> short, Shortint -> sbyte, Longint -> int (TP widths);
//   `^Integer` slots -> IntSlot accessors over the same state arrays;
//   open-array params -> plain arrays; 1-based tables keep index 0 empty.

using System.Text;

public static partial class Game
{
    public enum TFoe
    {
        WeakEnemy, WeakishEnemy, MediumEnemy, HardEnemy, VeryEasyEnemy,
        EasyEnemy, NotEasyEnemy, VeryHardEnemy
    }

    public enum TStat
    {
        Strength, Dexterity, Wisdom, MaxStrength, MaxDexterity, MaxWisdom
    }

    public enum TSkill
    {
        Parry, Kick, KickMinMana, KickMaxEnergy, Fleeing, FleeEnergy,
        Comparing, Cooking, CookingDisplayBug, Returning
    }

    public enum TGearItem
    {
        Armor, WornCount, ArmorRating, LuckRoll, Wearing,
        WeaponKind, ShieldKind
    }

    public enum TBagItem
    {
        Backpack, OldSword, SmallShield, Heart, Diploma, Pipe,
        Doughnut, Cookie, DryRation, Bread, BreadRoll, Sausage, Bigos,
        ManaBottle, Tracksuit, SpikedSuit, Cassette, CompareScroll,
        TeleportPill, LuckyLeaf, ReturnScroll, Beer
    }

    public enum TMonster
    {
        CageStrong, CageWeakling, CageAgile, CageTough, CageHuge,
        Woodworm, Fly, Bacterium, Snail, Beetle, Cockroach, Ant,
        Spider, Boar, Rat, Fox, Partridge, Hare, Mongrel, Eagle,
        Deer, Elephant, Lion, Giraffe, Camel, Ostrich, Boa, Wolf,
        Bison, Panther, Gladiator, Warrior, Tamer, Dachshund, Shepherd,
        Spaniel, Poodle, Pooch, Cabman, Vendor, Sweeper, Drunkard,
        SickMan, Kid, Lunatic, Listener, Fan, Man, Policeman, Bouncer,
        OldMan, Gorilla, Reporter, Guitarist, Drummer, Keyboardist,
        Liroy, Minibarman, Fatso, DJ, Pedal, Couple, Maciek, Sorrel,
        Daisy, Clover, SowThistle, Dandelion, Rose, Blackberry, Thistle,
        Gooseberry, Raspberry, Grass, Duncan
    }

    public enum TGainBonus { gbNone, gbMana, gbCassette }
    public enum TTakeEffect { teNone, teDiploma, tePipe }
    public enum TSheetKind { ikShown, ikDiv10, ikPlain }
    public enum TMazeDir { mdPolodnie, mdPolnoc, mdWschod, mdZachod }
    public enum TStreetKind { skDog, skFolk }

    public sealed class TPlayer
    {
        public short[] Stats = new short[(int)TStat.MaxWisdom + 1];
        public short Energy, MaxEnergy;
        public short Mana, MaxMana;
        public short Practice;
        public sbyte Level;
        public short Experience;
        public int Money;
    }

    public sealed class TSkills
    {
        public short[] Values = new short[(int)TSkill.Returning + 1];
    }

    public sealed class TGear
    {
        public short[] Values = new short[(int)TGearItem.ShieldKind + 1];
        public string WornGarment = "";
    }

    public sealed class TBag
    {
        public short Count;
        public short Limit;
        public int Permit;
        public short[] Items = new short[(int)TBagItem.Beer + 1];
    }

    public sealed class TCombat
    {
        public short EnemyEnergy, EnemyDexterity, EnemyStrength;
        public short EnemyDodged, PlayerDodged;
        public short DodgeChance, Damage;
        public short Fled;
        public short FightExperience;
        public sbyte FireballCharges, PoisonCharges, PoisonRounds;
        public short Loot, Roll;
    }

    public sealed class TWorld
    {
        public short[] Monsters = new short[(int)TMonster.Duncan + 1];
        public short Door, OldElf, Quest, QuestKills;
        public sbyte DuncanQuest;
    }

    public sealed class TSession
    {
        public short Room, OuterRoom, InArena, MoveGuard;
        public short CheckpointStage, SavedFlag, SleepHours, Overall;
        public bool ExitRequested;
        public SaveFile SaveFile = new();
    }

    public sealed class TSideRoom
    {
        public short room, dest;
        public string direction = "", exitDesc = "";
        public TSideRoom(short room, short dest, string direction, string exitDesc)
        {
            this.room = room; this.dest = dest;
            this.direction = direction; this.exitDesc = exitDesc;
        }
    }

    public sealed class TQuestOffer
    {
        public string cmd = "", line = "";
        public short quest, kills, price;
        public TQuestOffer(string cmd, string line, short quest, short kills, short price)
        {
            this.cmd = cmd; this.line = line;
            this.quest = quest; this.kills = kills; this.price = price;
        }
    }

    public sealed class TMovement
    {
        public string cmd = "";
        public short dest;
    }

    public sealed class TBand
    {
        public short lo, hi, Base, perUnit;
        public TBand(short lo, short hi, short Base, short perUnit)
        {
            this.lo = lo; this.hi = hi; this.Base = Base; this.perUnit = perUnit;
        }
    }

    // A Pascal `^Integer` slot. Every such slot points into a known
    // state array, so an accessor pair preserves the aliasing exactly.
    public sealed class IntSlot
    {
        public Func<short> Get;
        public Action<short> Set;
        public IntSlot(Func<short> get, Action<short> set) { Get = get; Set = set; }
        public static IntSlot Into(short[] array, int index) =>
            new(() => array[index], v => array[index] = v);
    }

    public sealed class TTranceItem
    {
        public IntSlot item;
        public string line = "";
        public TTranceItem(IntSlot item, string line) { this.item = item; this.line = line; }
    }

    public sealed class TSpawnedAt
    {
        public TMonster monster;
        public short room;
        public TSpawnedAt(TMonster monster, short room) { this.monster = monster; this.room = room; }
    }

    public sealed class TCheckpoint
    {
        public short exp, stage, next, loStr, hiStr;
        public TCheckpoint(short exp, short stage, short next, short loStr, short hiStr)
        {
            this.exp = exp; this.stage = stage; this.next = next;
            this.loStr = loStr; this.hiStr = hiStr;
        }
    }

    public sealed class TTrainable
    {
        public string cmd = "";
        public TStat stat, maxStat;
        public short minPractice, cost;
        public string cmdForm = "", nounForm = "";
    }

    public sealed class TCommandEntry
    {
        public string cmd = "";
        public Action run = () => { };
        public TCommandEntry(string cmd, Action run) { this.cmd = cmd; this.run = run; }
    }

    public static TPlayer Player = new();
    public static TSkills Skills = new();
    public static TGear Gear = new();
    public static TBag Bag = new();
    public static TCombat Combat = new();
    public static TWorld World = new();
    public static TSession Session = new();
    public static string Command = "";

    public static Action DispatchRooms = () => { };

    // Deterministic PRNG for test runs. The test oracle (FPC dev build)
    // replaces System.Random with xorshift64* seeded from BOMBKI_SEED,
    // so this ports that exact scheme: same state init, same reduction.
    // Unset BOMBKI_SEED keeps genuine Pascal behavior (clock-seeded,
    // every run differs); Randomize re-reads the variable every call.
    private const ulong BombkiDefaultSeed = 0x9E3779B97F4A7C15;
    private const ulong XorshiftMult = 2685821657736338717;
    private static ulong _bombkiState = BombkiDefaultSeed;

    private static ulong ParseSeed(string? text)
    {
        if (text is null)
            return 0;
        int i = 0;
        while (i < text.Length && (text[i] == ' ' || text[i] == '\t'))
            i++;
        ulong numberBase = 10;
        if (i + 1 < text.Length && text[i] == '0' && (text[i + 1] == 'x' || text[i + 1] == 'X'))
        {
            numberBase = 16;
            i += 2;
        }
        ulong value = 0;
        int digits = 0;
        for (; i < text.Length; i++)
        {
            char c = text[i];
            ulong digit;
            if (c >= '0' && c <= '9')
                digit = (ulong)(c - '0');
            else if (numberBase == 16 && c >= 'a' && c <= 'f')
                digit = (ulong)(c - 'a') + 10;
            else if (numberBase == 16 && c >= 'A' && c <= 'F')
                digit = (ulong)(c - 'A') + 10;
            else
                break;
            unchecked { value = value * numberBase + digit; }
            digits++;
        }
        return digits == 0 ? 0 : value;
    }

    public static void Randomize()
    {
        string? raw = Environment.GetEnvironmentVariable("BOMBKI_SEED");
        if (raw is null)
        {
            _bombkiState = (ulong)DateTime.UtcNow.Ticks;
            return;
        }
        ulong parsed = ParseSeed(raw);
        _bombkiState = parsed == 0 ? BombkiDefaultSeed : parsed;
    }

    private static uint NextBelow(uint bound)
    {
        ulong x = _bombkiState;
        x ^= x >> 12;
        x ^= x << 25;
        x ^= x >> 27;
        _bombkiState = x;
        return (uint)(((x * XorshiftMult) >> 32) % bound);
    }

    // Random(0) returns 0 without advancing, as FPC documents.
    public static short Roll(int spread)
    {
        if (spread <= 0)
            return 0;
        return (short)NextBelow((uint)spread);
    }

    public const int MaxInt = 32767;
    public const int ItemUnit = 10;
    public const int SaveLevelBias = 0x17;
    public const int SaveEnergyBias = 0x28;
    public const short SaveStrengthBias = 24;
    public const short SaveDexterityBias = 12;
    public const int SaveStatScale = 4;

    public static readonly TSideRoom[] SideRooms = {
        null!,
        new(RoomSchoolSide1, RoomLost, CmdWschod, CmdMudSzkola2),
        new(RoomSchoolSide3, RoomLost, CmdPolnoc, CmdMudSzkola2),
        new(RoomSchoolSide2, RoomLost, CmdPolodnie, CmdMudSzkola2),
        new(RoomSchoolSide4, RoomLost, CmdGora, CmdMudSzkola2),
    };

    public static readonly TQuestOffer[] QuestOffers = {
        null!,
        new(CmdKup + CmdLatwyQuest, TxtRoom100_09, 1, 75, 200),
        new(CmdKup + CmdPrzecietnyQuest, TxtRoom100_10, 2, 200, 100),
        new(CmdKup + CmdTrudnyQuest, TxtRoom100_11, 3, 200, 50),
    };

    public static readonly TTranceItem[] TranceItems = {
        null!,
        new(IntSlot.Into(Bag.Items, (int)TBagItem.OldSword), TxtStart_07),
        new(IntSlot.Into(Bag.Items, (int)TBagItem.SmallShield), TxtTranceItemLabel + TxtStart_08),
        new(IntSlot.Into(Bag.Items, (int)TBagItem.Heart), TxtTranceItemLabel + TxtStart_09),
        new(IntSlot.Into(Bag.Items, (int)TBagItem.Diploma), TxtTranceItemLabel + TxtStart_10),
        new(IntSlot.Into(Bag.Items, (int)TBagItem.Pipe), TxtStart_11),
    };

    public static void WriteExits(string[] items)
    {
        Out.WriteLine(items[0]);
        for (int i = 1; i <= (items.Length - 1) / 2; i++)
        {
            Out.Write(items[2 * i - 1]);
            Out.WriteLine(items[2 * i]);
        }
    }

    public static TMovement Go(string cmd, short dest)
    {
        TMovement result = new();
        result.cmd = cmd;
        result.dest = dest;
        return result;
    }

    public static void MoveWhen(TMovement[] moves)
    {
        for (int i = 0; i < moves.Length; i++)
            if (Command == moves[i].cmd)
                Session.Room = moves[i].dest;
    }

    public static bool MoveWhenOrExit(TMovement[] moves)
    {
        if (Command == CmdWyjscie)
            return true;
        MoveWhen(moves);
        return false;
    }

    public static bool MoveWhenOrQuit(TMovement[] moves)
    {
        bool leave = MoveWhenOrExit(moves);
        if (leave)
            Session.ExitRequested = true;
        return leave;
    }

    // Open-array params re-base to 0; static 1-based tables are
    // sliced ([1..]) at such call sites to match.
    public static void WriteLines(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
            Out.WriteLine(lines[i]);
    }

    public static void TryLoot(ref short slot, short threshold, string foundMsg)
    {
        if (slot == 0)
        {
            Combat.Roll = Roll(20);
            if (Combat.Roll < threshold)
            {
                slot = Session.Room;
                Out.WriteLine(TxtShared_08 + foundMsg);
            }
        }
    }

    public static void TryCageLoot(short threshold)
    {
        TryLoot(ref Bag.Items[(int)TBagItem.OldSword], threshold, TxtShared_10);
        TryLoot(ref Bag.Items[(int)TBagItem.SmallShield], threshold, TxtShared_11);
        TryLoot(ref Bag.Items[(int)TBagItem.Heart], 6, TxtShared_12);
    }

    public static void TranceItemLine(short itemRoom, string line)
    {
        if (itemRoom == Session.OuterRoom)
            Out.WriteLine(line);
    }

    public static void ShowIntro()
    {
        short tone, step;
        Crt.ClrScr();
        Player.MaxEnergy = 50;
        WriteLines(TxtShowIntroLines1[1..]);
        tone = 100;
        step = 100;
        do
        {
            Crt.Sound(tone);
            Crt.Delay(100);
            if (tone == 600) step = -100;
            tone = (short)(tone + step);
            if (tone == 100)
            {
                Out.WriteLine(TxtShowIntro_15);
                step = 100;
            }
        } while (!((tone == 100) && Crt.KeyPressed()));
        Crt.NoSound();
    }

    public static void BuyQuest(string cmd, string line, short quest, short kills, short price)
    {
        if ((Command == cmd) && (Player.Money >= price))
        {
            Out.WriteLine(line);
            World.Quest = quest;
            World.QuestKills = kills;
            Player.Money = Player.Money - price;
        }
    }

    public static readonly TBand[] PosExpBands = { null!,
        new(1, 2, 10, 0), new(3, 3, 19, 0), new(4, 6, 9, 0),
        new(7, 9, 8, 0), new(10, 12, 7, 0), new(13, 15, 6, 0),
        new(16, 18, 5, 0), new(19, 21, 4, 0), new(22, 24, 3, 0),
        new(25, 27, 2, 0), new(28, 30, 1, 0), new(31, MaxInt, 0, 0),
    };

    public static readonly TBand[] NegExpBands = { null!,
        new(2, 2, 12, 0), new(4, 4, 13, 0), new(6, 6, 14, 0),
        new(8, 8, 15, 0), new(10, 10, 16, 0), new(12, 12, 17, 0),
        new(14, 14, 18, 0), new(16, 16, 19, 0), new(18, 18, 20, 0),
        new(21, MaxInt, 21, 0),
    };

    public static short BandAmount(TBand[] bands, short v)
    {
        for (int bi = 0; bi < bands.Length; bi++)
        {
            TBand band = bands[bi];
            if ((v >= band.lo) && (v <= band.hi))
                return (short)(band.Base + band.perUnit * v);
        }
        return 0;
    }

    public static short FightExpForEnergy(short diff)
    {
        if (diff > 0)
            return BandAmount(PosExpBands[1..], diff);
        else if (diff < 0)
            return BandAmount(NegExpBands[1..], (short)(-diff));
        else
            return 11;
    }

    public static short FightExpForStrength(short diff)
    {
        if (diff > 0)
        {
            if (diff >= 10) return 1;
            else return (short)(11 - diff);
        }
        else if (diff < 0)
        {
            diff = (short)(-diff);
            if (diff >= 10) return 21;
            else return (short)(11 + diff);
        }
        else
            return 11;
    }

    public static short FightExpForDexterity(short diff)
    {
        short step;
        if (diff > 0)
        {
            step = (short)((diff + 1) / 2);
            if (step >= 10) return 1;
            else return (short)(11 - step);
        }
        else if (diff < 0)
        {
            diff = (short)(-diff);
            step = (short)((diff + 1) / 2);
            if (step >= 10) return 21;
            else return (short)(11 + step);
        }
        else
            return 11;
    }

    public sealed class TDodgeBound
    {
        public short limit, player, enemy;
        public TDodgeBound(short limit, short player, short enemy)
        {
            this.limit = limit; this.player = player; this.enemy = enemy;
        }
    }

    public static readonly TDodgeBound[] DodgeBounds = { null!,
        new(30, 12, 60), new(27, 13, 58), new(24, 14, 56),
        new(20, 15, 54), new(18, 16, 52), new(16, 17, 51),
        new(12, 18, 48), new(9, 19, 46), new(6, 20, 42),
        new(4, 21, 40), new(2, 22, 38), new(0, 23, 35),
    };

    public static TDodgeBound DodgeBoundRow(short chance)
    {
        int i = 1;
        while ((i < DodgeBounds.Length - 1) && (chance <= DodgeBounds[i].limit))
            i++;
        return DodgeBounds[i];
    }

    public static short DodgeBound(short chance) => DodgeBoundRow(chance).player;
    public static short EnemyDodgeBound(short chance) => DodgeBoundRow(chance).enemy;

    public static short DexterityBonus(short value)
    {
        if (value >= 8) return (short)(value / 2);
        else return 0;
    }

    public static short ParryBonus(short value)
    {
        short bonus = 0;
        if (value > 75) bonus++;
        if (value > 50) bonus++;
        return bonus;
    }

    public static short KickBonus(short value)
    {
        short bonus = 0;
        if (value > 70) bonus += 2;
        if (value > 10) bonus += 2;
        return bonus;
    }

    public static void FightDog()
    {
        Combat.EnemyEnergy = 10;
        Combat.EnemyDexterity = 10;
        Combat.EnemyStrength = 5;
        DoFight();
        Combat.Loot = Roll(15);
        Out.WriteLine(TxtShared_08 + Combat.Loot + TxtFightDog_01);
        Player.Money = Player.Money + Combat.Loot;
        TryLoot(ref Bag.Items[(int)TBagItem.Heart], 5, TxtFightDog_02);
    }

    public static readonly TSpawnedAt[] CageInits = { null!,
        new(TMonster.CageWeakling, RoomCageWeakling),
        new(TMonster.CageStrong, RoomCageStrong),
        new(TMonster.CageAgile, RoomCageAgile),
        new(TMonster.CageTough, RoomCageTough),
        new(TMonster.CageHuge, RoomCageHuge),
    };

    public static readonly TSpawnedAt[] PubSpawns = { null!,
        new(TMonster.Minibarman, RoomPub),
        new(TMonster.Fatso, RoomPub),
        new(TMonster.DJ, RoomBackstage),
        new(TMonster.Pedal, RoomBar),
        new(TMonster.Couple, RoomBar),
        new(TMonster.Maciek, RoomBar),
    };

    public sealed class TSpot
    {
        public TMonster where;
        public string text = "";
        public TSpot(TMonster where, string text) { this.where = where; this.text = text; }
    }

    public static readonly TSpot[] PlantSpots = { null!,
        new(TMonster.Sorrel, TxtDescribePlants_01),
        new(TMonster.Daisy, TxtDescribePlants_02),
        new(TMonster.Clover, TxtDescribePlants_03),
        new(TMonster.SowThistle, TxtDescribePlants_04),
        new(TMonster.Dandelion, TxtDescribePlants_05),
        new(TMonster.Rose, TxtDescribePlants_06),
        new(TMonster.Thistle, TxtDescribePlants_07),
        new(TMonster.Blackberry, TxtDescribePlants_08),
        new(TMonster.Gooseberry, TxtDescribePlants_09),
        new(TMonster.Raspberry, TxtDescribePlants_10),
        new(TMonster.Grass, TxtDescribePlants_11),
    };

    public static readonly TSpot[] BeastSpots = { null!,
        new(TMonster.Woodworm, TxtDescribeBeasts_01),
        new(TMonster.Fly, TxtDescribeBeasts_02),
        new(TMonster.Bacterium, TxtDescribeBeasts_03),
        new(TMonster.Snail, TxtDescribeBeasts_04),
        new(TMonster.Beetle, TxtDescribeBeasts_05),
        new(TMonster.Cockroach, TxtDescribeBeasts_06),
        new(TMonster.Ant, TxtDescribeBeasts_07),
        new(TMonster.Spider, TxtDescribeBeasts_08),
        new(TMonster.Boar, TxtDescribeBeasts_09),
        new(TMonster.Rat, TxtDescribeBeasts_10),
        new(TMonster.Fox, TxtDescribeBeasts_11),
        new(TMonster.Partridge, TxtDescribeBeasts_12),
        new(TMonster.Hare, TxtDescribeBeasts_13),
        new(TMonster.Mongrel, TxtDescribeBeasts_14),
        new(TMonster.Eagle, TxtDescribeBeasts_15),
        new(TMonster.Deer, TxtDescribeBeasts_16),
        new(TMonster.Elephant, TxtDescribeBeasts_17),
        new(TMonster.Lion, TxtDescribeBeasts_18),
        new(TMonster.Giraffe, TxtDescribeBeasts_19),
        new(TMonster.Camel, TxtDescribeBeasts_20),
        new(TMonster.Ostrich, TxtDescribeBeasts_21),
        new(TMonster.Boa, TxtDescribeBeasts_22),
        new(TMonster.Wolf, TxtDescribeBeasts_23),
        new(TMonster.Bison, TxtDescribeBeasts_24),
        new(TMonster.Panther, TxtDescribeBeasts_25),
        new(TMonster.Gladiator, TxtDescribeBeasts_26),
        new(TMonster.Warrior, TxtDescribeBeasts_27),
        new(TMonster.Tamer, TxtDescribeBeasts_28),
    };

    public static readonly TSpot[] StreetSpots = { null!,
        new(TMonster.Dachshund, TxtDescribeStreet_01),
        new(TMonster.Shepherd, TxtDescribeStreet_02),
        new(TMonster.Pooch, TxtDescribeStreet_03),
        new(TMonster.Spaniel, TxtDescribeStreet_04),
        new(TMonster.Poodle, TxtDescribeStreet_05),
        new(TMonster.Cabman, TxtDescribeStreet_06),
        new(TMonster.Vendor, TxtDescribeStreet_07),
        new(TMonster.Sweeper, TxtDescribeStreet_08),
        new(TMonster.Drunkard, TxtDescribeStreet_09),
        new(TMonster.SickMan, TxtDescribeStreet_10),
    };

    public static readonly TSpot[] StageSpots = { null!,
        new(TMonster.Guitarist, TxtDescribeStage_01),
        new(TMonster.Drummer, TxtDescribeStage_02),
        new(TMonster.Keyboardist, TxtDescribeStage_03),
        new(TMonster.Liroy, TxtDescribeStage_04),
    };

    public static readonly TSpot[] CrowdSpots = { null!,
        new(TMonster.Kid, TxtDescribeCrowd_01),
        new(TMonster.Lunatic, TxtDescribeCrowd_02),
        new(TMonster.Listener, TxtDescribeCrowd_03),
        new(TMonster.Fan, TxtDescribeCrowd_04),
        new(TMonster.Man, TxtDescribeCrowd_05),
        new(TMonster.Policeman, TxtDescribeCrowd_06),
        new(TMonster.Bouncer, TxtDescribeCrowd_07),
        new(TMonster.Gorilla, TxtDescribeCrowd_08),
        new(TMonster.OldMan, TxtDescribeCrowd_09),
        new(TMonster.Reporter, TxtDescribeCrowd_10),
    };

    public static void SpawnAt(TSpawnedAt[] placements)
    {
        for (int si = 0; si < placements.Length; si++)
            World.Monsters[(int)placements[si].monster] = placements[si].room;
    }

    public static void SpawnGroup(int[] targets, short spread, short Base)
    {
        for (int i = 0; i < targets.Length; i++)
            World.Monsters[targets[i]] = (short)(Roll(spread) + Base);
    }

    public static bool PastLimit(int[] targets, short limit)
    {
        for (int li = 0; li < targets.Length; li++)
            if (World.Monsters[targets[li]] <= limit)
                return false;
        return true;
    }

    private static int[] MonstersOf(params TMonster[] monsters)
    {
        int[] result = new int[monsters.Length];
        for (int i = 0; i < monsters.Length; i++)
            result[i] = (int)monsters[i];
        return result;
    }

    public static void SpawnMonsters()
    {
        do
        {
            SpawnGroup(MonstersOf(TMonster.Bacterium, TMonster.Snail, TMonster.Beetle, TMonster.Cockroach, TMonster.Ant,
                TMonster.Spider, TMonster.Boar, TMonster.Fox, TMonster.Rat, TMonster.Partridge), 38, RoomCityCentre);
        } while (!PastLimit(MonstersOf(TMonster.Bacterium, TMonster.Snail, TMonster.Beetle, TMonster.Cockroach, TMonster.Ant,
            TMonster.Spider, TMonster.Boar, TMonster.Fox, TMonster.Rat, TMonster.Partridge), RoomArenaEntrance));
        do
        {
            SpawnGroup(MonstersOf(TMonster.Deer, TMonster.Eagle, TMonster.Hare, TMonster.Mongrel, TMonster.Woodworm, TMonster.Fly), 38, RoomCityCentre);
        } while (!PastLimit(MonstersOf(TMonster.Deer, TMonster.Eagle, TMonster.Hare, TMonster.Mongrel, TMonster.Woodworm, TMonster.Fly), RoomArenaEntrance));
        do
        {
            SpawnGroup(MonstersOf(TMonster.Elephant, TMonster.Lion, TMonster.Giraffe, TMonster.Camel, TMonster.Ostrich,
                TMonster.Boa, TMonster.Wolf, TMonster.Bison, TMonster.Panther), 38, RoomCityCentre);
        } while (!PastLimit(MonstersOf(TMonster.Elephant, TMonster.Lion, TMonster.Giraffe, TMonster.Camel, TMonster.Ostrich,
            TMonster.Boa, TMonster.Wolf, TMonster.Bison, TMonster.Panther), RoomArenaEntrance));
        do
        {
            SpawnGroup(MonstersOf(TMonster.Gladiator, TMonster.Warrior, TMonster.Tamer), 38, RoomCityCentre);
        } while (!PastLimit(MonstersOf(TMonster.Gladiator, TMonster.Warrior, TMonster.Tamer), RoomArenaEntrance));
        do
        {
            SpawnGroup(MonstersOf(TMonster.Dachshund, TMonster.Shepherd, TMonster.Pooch, TMonster.Spaniel, TMonster.Poodle), 7, RoomCityCentre);
        } while (!PastLimit(MonstersOf(TMonster.Dachshund, TMonster.Shepherd, TMonster.Pooch, TMonster.Spaniel, TMonster.Poodle), 19));
        do
        {
            SpawnGroup(MonstersOf(TMonster.Cabman, TMonster.Vendor, TMonster.Sweeper, TMonster.Drunkard, TMonster.SickMan), 7, RoomCityCentre);
        } while (!PastLimit(MonstersOf(TMonster.Cabman, TMonster.Vendor, TMonster.Sweeper, TMonster.Drunkard, TMonster.SickMan), 19));
        do
        {
            SpawnGroup(MonstersOf(TMonster.Kid), 7, RoomValley);
            SpawnGroup(MonstersOf(TMonster.Lunatic, TMonster.Listener, TMonster.Fan, TMonster.Man), 8, RoomValley);
        } while (!PastLimit(MonstersOf(TMonster.Kid, TMonster.Lunatic, TMonster.Listener, TMonster.Fan, TMonster.Man), RoomValley));
        do
        {
            SpawnGroup(MonstersOf(TMonster.Policeman, TMonster.Bouncer, TMonster.Gorilla), 9, RoomValley);
            SpawnGroup(MonstersOf(TMonster.OldMan), 8, RoomValley);
            SpawnGroup(MonstersOf(TMonster.Reporter), 7, RoomValley);
        } while (!PastLimit(MonstersOf(TMonster.Policeman, TMonster.Bouncer, TMonster.Gorilla, TMonster.OldMan, TMonster.Reporter), RoomValley));
        SpawnGroup(MonstersOf(TMonster.Guitarist, TMonster.Drummer, TMonster.Keyboardist, TMonster.Liroy), 3, RoomStage1);
        SpawnAt(PubSpawns[1..]);
        World.Door = 1;
        World.OldElf = 1;
        do
        {
            SpawnGroup(MonstersOf(TMonster.Sorrel, TMonster.Clover, TMonster.Daisy, TMonster.SowThistle, TMonster.Dandelion,
                TMonster.Rose, TMonster.Thistle, TMonster.Raspberry, TMonster.Gooseberry, TMonster.Blackberry,
                TMonster.Grass), 8, RoomIvy1);
            SpawnGroup(MonstersOf(TMonster.Duncan), 8, RoomIvy1);
        } while (!((World.Monsters[(int)TMonster.Sorrel] < RoomCavePath) && (World.Monsters[(int)TMonster.Daisy] < RoomCavePath)
            && (World.Monsters[(int)TMonster.Clover] < RoomCavePath) && (World.Monsters[(int)TMonster.SowThistle] < RoomCavePath)
            && (World.Monsters[(int)TMonster.Dandelion] < RoomCavePath) && (World.Monsters[(int)TMonster.Rose] < RoomCavePath)
            && (World.Monsters[(int)TMonster.Thistle] < RoomCavePath) && (World.Monsters[(int)TMonster.Raspberry] < RoomCavePath)));
    }

    public static void TrainAttribute(ref short Attribute, ref short MaxAttribute, short MinPractice, short PracticeCost, string cmdForm, string nounForm)
    {
        if ((Player.Practice > MinPractice) && (Attribute < MaxAttribute))
        {
            Attribute = (short)(Attribute + 1);
            Player.Practice = (short)(Player.Practice - PracticeCost);
            Out.WriteLine(TxtTrainAttribute_01 + cmdForm + TxtTrainAttribute_02 + Attribute + TxtTrainAttribute_03 + nounForm + TxtTrainAttribute_04 + Player.Practice + TxtShared_19);
        }
    }

    public static readonly TTrainable[] Trainables = { null!,
        new() { cmd = CmdTrenuj2 + CmdSila, stat = TStat.Strength, maxStat = TStat.MaxStrength, minPractice = 2, cost = 3, cmdForm = CmdSile, nounForm = CmdSily },
        new() { cmd = CmdTrenuj2 + CmdZrecznosc, stat = TStat.Dexterity, maxStat = TStat.MaxDexterity, minPractice = 1, cost = 2, cmdForm = CmdZrecznosc, nounForm = CmdZrecznosci },
        new() { cmd = CmdTrenuj2 + CmdMadrosc, stat = TStat.Wisdom, maxStat = TStat.MaxWisdom, minPractice = 2, cost = 3, cmdForm = CmdMadrosc, nounForm = CmdMadrosci },
    };

    public static void HandleTrain()
    {
        if (Command == CmdTrenuj)
            Out.WriteLine(TxtHandleTrain_01);
        for (int ti = 1; ti < Trainables.Length; ti++)
        {
            TTrainable t = Trainables[ti];
            if (Command == t.cmd)
                TrainAttribute(ref Player.Stats[(int)t.stat], ref Player.Stats[(int)t.maxStat], t.minPractice, t.cost, t.cmdForm, t.nounForm);
        }
    }

    public static void DescribePlants()
    {
        for (int i = 1; i < PlantSpots.Length; i++)
            if (World.Monsters[(int)PlantSpots[i].where] == Session.Room)
                Out.WriteLine(PlantSpots[i].text);
        if (Session.Room == World.Monsters[(int)TMonster.Duncan])
            Out.WriteLine(TxtDescribePlants_12);
    }

    public static void CompositeFight(short EnemyEnergyBase, short EnemyEnergyRand, short EnemyDexterityBase, short EnemyDexterityRand, short EnemyStrengthBase, short EnemyStrengthRand,
        short LootBase, short LootRand, short HeartChance, bool RequireEnergy, bool AntDoughnut)
    {
        if (EnemyEnergyRand > 0) Combat.EnemyEnergy = (short)(Roll(EnemyEnergyRand) + EnemyEnergyBase); else Combat.EnemyEnergy = EnemyEnergyBase;
        if (EnemyDexterityRand > 0) Combat.EnemyDexterity = (short)(Roll(EnemyDexterityRand) + EnemyDexterityBase); else Combat.EnemyDexterity = EnemyDexterityBase;
        if (EnemyStrengthRand > 0) Combat.EnemyStrength = (short)(Roll(EnemyStrengthRand) + EnemyStrengthBase); else Combat.EnemyStrength = EnemyStrengthBase;
        DoFight();
        if ((Combat.Fled == 0) && (!RequireEnergy || (Player.Energy > 0)))
        {
            Combat.Loot = (short)(Roll(LootRand) + LootBase);
            Out.WriteLine(TxtShared_08 + Combat.Loot + TxtCompositeFight_01);
            Player.Money = Player.Money + Combat.Loot;
            if (AntDoughnut)
            {
                if (Command == CmdZabij + CmdMrowka)
                {
                    Combat.Roll = Roll(10);
                    if ((Combat.Roll < 7) && (Bag.Items[(int)TBagItem.Doughnut] != -ItemUnit))
                    {
                        Out.WriteLine(TxtShared_08 + TxtCompositeFight_02);
                        Bag.Items[(int)TBagItem.Doughnut] = -ItemUnit;
                        Bag.Count = (short)(Bag.Count + 1);
                    }
                }
            }
            else if ((HeartChance > 0) && (Bag.Items[(int)TBagItem.Heart] == 0))
            {
                Combat.Roll = Roll(20);
                if ((Combat.Roll < HeartChance) && (Bag.Items[(int)TBagItem.Heart] != -ItemUnit))
                {
                    Out.WriteLine(TxtShared_08 + TxtCompositeFight_03);
                    Bag.Items[(int)TBagItem.Heart] = -ItemUnit;
                    Bag.Count = (short)(Bag.Count + 1);
                }
            }
        }
    }

    public sealed class TFoeStats
    {
        public short energyBase, energyRand, dexterityBase, dexterityRand;
        public short strengthBase, strengthRand, lootBase, lootRand, heartChance;
        public bool requireEnergy, antDoughnut;
    }

    public static readonly TFoeStats[] Foes = {
        new() { energyBase = 1, energyRand = 0, dexterityBase = 1, dexterityRand = 0,
            strengthBase = 2, strengthRand = 0, lootBase = 0, lootRand = 3,
            heartChance = 0, requireEnergy = false, antDoughnut = true },
        new() { energyBase = 34, energyRand = 3, dexterityBase = 8, dexterityRand = 4,
            strengthBase = 10, strengthRand = 0, lootBase = 10, lootRand = 21,
            heartChance = 5, requireEnergy = false, antDoughnut = false },
        new() { energyBase = 50, energyRand = 5, dexterityBase = 10, dexterityRand = 5,
            strengthBase = 13, strengthRand = 2, lootBase = 30, lootRand = 30,
            heartChance = 7, requireEnergy = false, antDoughnut = false },
        new() { energyBase = 60, energyRand = 6, dexterityBase = 12, dexterityRand = 3,
            strengthBase = 15, strengthRand = 3, lootBase = 20, lootRand = 40,
            heartChance = 7, requireEnergy = false, antDoughnut = false },
        new() { energyBase = 90, energyRand = 10, dexterityBase = 14, dexterityRand = 2,
            strengthBase = 17, strengthRand = 2, lootBase = 10, lootRand = 55,
            heartChance = 4, requireEnergy = true, antDoughnut = false },
        new() { energyBase = 100, energyRand = 20, dexterityBase = 16, dexterityRand = 0,
            strengthBase = 18, strengthRand = 2, lootBase = 30, lootRand = 30,
            heartChance = 4, requireEnergy = true, antDoughnut = false },
        new() { energyBase = 130, energyRand = 20, dexterityBase = 20, dexterityRand = 0,
            strengthBase = 19, strengthRand = 2, lootBase = 20, lootRand = 45,
            heartChance = 4, requireEnergy = true, antDoughnut = false },
        new() { energyBase = 72, energyRand = 3, dexterityBase = 13, dexterityRand = 2,
            strengthBase = 16, strengthRand = 2, lootBase = 15, lootRand = 50,
            heartChance = 5, requireEnergy = false, antDoughnut = false },
    };

    public static void Fight(TFoe foe)
    {
        TFoeStats f = Foes[(int)foe];
        CompositeFight(f.energyBase, f.energyRand, f.dexterityBase, f.dexterityRand,
            f.strengthBase, f.strengthRand, f.lootBase, f.lootRand, f.heartChance,
            f.requireEnergy, f.antDoughnut);
    }

    public static void KillBeast(string cmd, ref short beast, TFoe foe)
    {
        if ((Command == cmd) && (Session.Room == beast))
        {
            Fight(foe);
            beast = 0;
        }
    }

    public static void KillBeastUnlessFled(string cmd, ref short beast, TFoe foe)
    {
        if ((Command == cmd) && (Session.Room == beast))
        {
            Fight(foe);
            if (Combat.Fled == 0)
                beast = 0;
        }
    }

    public sealed class TCompareFoeRule
    {
        public IntSlot stat;
        public short lo, hi;
        public string line = "";
        public TCompareFoeRule(IntSlot stat, short lo, short hi, string line)
        {
            this.stat = stat; this.lo = lo; this.hi = hi; this.line = line;
        }
    }

    public static void CompareFoeRules(TCompareFoeRule[] rules)
    {
        for (int i = 0; i < rules.Length; i++)
        {
            TCompareFoeRule rule = rules[i];
            if ((rule.stat.Get() >= rule.lo) && (rule.stat.Get() <= rule.hi))
                Out.WriteLine(rule.line);
        }
    }

    private static IntSlot SessionOverall() =>
        new(() => Session.Overall, v => Session.Overall = v);
    private static IntSlot PlayerStrength() =>
        IntSlot.Into(Player.Stats, (int)TStat.Strength);

    public static readonly TCompareFoeRule[] BeastCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 15, TxtCompareFoe_02),
        new(PlayerStrength(), 16, 18, TxtEnemy + TxtCompareFoe_03),
        new(PlayerStrength(), 19, MaxInt, TxtCompareFoe_04),
    };

    public static readonly TCompareFoeRule[] BigBeastCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 15, TxtCompareFoe_05),
        new(PlayerStrength(), 16, 20, TxtCompareFoe_06),
        new(PlayerStrength(), 21, 22, TxtCompareFoe_07),
        new(PlayerStrength(), 23, MaxInt, TxtCompareFoe_08),
    };

    public static readonly TCompareFoeRule[] FighterCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 14, TxtCompareFoe_09),
        new(SessionOverall(), 15, 17, TxtCompareFoe_10),
        new(SessionOverall(), 18, 22, TxtCompareFoe_11),
        new(SessionOverall(), 23, 25, TxtCompareFoe_12),
        new(SessionOverall(), 26, MaxInt, TxtCompareFoe_13),
    };

    public static readonly TCompareFoeRule[] DrunkCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 23, TxtCompareFoe_14),
        new(SessionOverall(), 24, 25, TxtCompareFoe_15),
        new(SessionOverall(), 26, 28, TxtCompareFoe_16),
        new(SessionOverall(), 29, MaxInt, TxtCompareFoe_17),
        new(SessionOverall(), 35, MaxInt, TxtCompareFoe_18),
    };

    public static readonly TCompareFoeRule[] MusicianCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 20, TxtCompareFoe_19),
        new(SessionOverall(), 23, 27, TxtCompareFoe_20),
        new(SessionOverall(), 28, MaxInt, TxtCompareFoe_21),
    };

    public static readonly TCompareFoeRule[] IvyCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 19, TxtCompareFoe_22),
        new(SessionOverall(), 20, 25, TxtCompareFoe_23),
        new(SessionOverall(), 26, 29, TxtCompareFoe_24),
        new(SessionOverall(), 30, MaxInt, TxtCompareFoe_25),
        new(SessionOverall(), 36, MaxInt, TxtCompareFoe_26),
    };

    public static readonly TCompareFoeRule[] LiroyCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 27, TxtCompareFoe_27),
        new(SessionOverall(), 28, 32, TxtCompareFoe_28),
        new(SessionOverall(), 33, 43, TxtCompareFoe_29),
        new(SessionOverall(), 44, 47, TxtCompareFoe_30),
        new(SessionOverall(), 48, MaxInt, TxtCompareFoe_31),
    };

    public static readonly TCompareFoeRule[] PokrzywaCompareRules = { null!,
        new(SessionOverall(), -MaxInt - 1, 39, TxtCompareFoe_32),
        new(SessionOverall(), 40, MaxInt, TxtCompareFoe_33),
    };

    public static void CompareFoe()
    {
        if (Player.Mana > 9)
        {
            Combat.Roll = Roll(100);
            Out.WriteLine(TxtCompareFoe_01);
            Command = Crt.ReadLn();
            Session.Overall = (short)(Player.Level + Player.Stats[(int)TStat.Strength] + DexterityBonus(Player.Stats[(int)TStat.Dexterity]) + ParryBonus(Skills.Values[(int)TSkill.Parry]) + KickBonus(Skills.Values[(int)TSkill.Kick]));
            if ((Command == CmdDzik) || (Command == CmdSzczur) || (Command == CmdLis) ||
                (Command == CmdKuropatwa) || (Command == CmdZajac) || (Command == CmdWilczur) ||
                (Command == CmdOrzel) || (Command == CmdSarna) || (Command == CmdDziecko) || (Command == CmdDziadek))
                CompareFoeRules(BeastCompareRules[1..]);
            if ((Command == CmdSlon) || (Command == CmdLew) || (Command == CmdZyrafa) || (Command == CmdWielblad) ||
                (Command == CmdStrus) || (Command == CmdBoa) || (Command == CmdWilk) || (Command == CmdBizon) ||
                (Command == CmdPantera) || (Command == CmdWariat) || (Command == CmdSluchacz) || (Command == CmdFan) ||
                (Command == CmdCzlowiek) || (Command == CmdReporter))
                CompareFoeRules(BigBeastCompareRules[1..]);
            if ((Command == CmdGladiator) || (Command == CmdWojownik) || (Command == CmdTrener) ||
                (Command == CmdPolicjant) || (Command == CmdGoryl) || (Command == CmdOchroniarz))
                CompareFoeRules(FighterCompareRules[1..]);
            if ((Command == CmdMiniBarman) || (Command == CmdGrubas) || (Command == CmdDJ))
                CompareFoeRules(DrunkCompareRules[1..]);
            if ((Command == CmdTaksowkarz) || (Command == CmdSprzedawca) || (Command == CmdGitarzysta) || (Command == CmdPerkusista) ||
                (Command == CmdOrganista) || (Command == CmdZebrak) || (Command == CmdPijak) || (Command == CmdZamiatacz) || (Command == CmdSzczaw) ||
                (Command == CmdStokrotka) || (Command == CmdKoniczynka) || (Command == CmdMlecz) || (Command == CmdDmuchawiec))
                CompareFoeRules(MusicianCompareRules[1..]);
            if ((Command == CmdPedal) || (Command == CmdPara) || (Command == CmdMaciek) || (Command == CmdRoza) ||
                (Command == CmdMalina) || (Command == CmdTrawa) || (Command == CmdAgrest) || (Command == CmdJezyna))
                CompareFoeRules(IvyCompareRules[1..]);
            if (Command == CmdLiroy)
                CompareFoeRules(LiroyCompareRules[1..]);
            if (Command == CmdPokrzywa)
                CompareFoeRules(PokrzywaCompareRules[1..]);
            if ((Command == CmdSpaniel) || (Command == CmdOwczarek) || (Command == CmdPiesek) || (Command == CmdJamnik) || (Command == CmdPudel))
                Out.WriteLine(TxtCompareFoe_34);
            if ((Combat.Roll < 3) && (Skills.Values[(int)TSkill.Comparing] < 100))
            {
                Out.WriteLine(TxtFightStars17 + TxtLearnVerb + TxtCompareFoe_35);
                Skills.Values[(int)TSkill.Comparing]++;
                Player.Experience = (short)(Player.Experience + 5);
            }
            if (Command == CmdBakteria)
            {
                Out.WriteLine(TxtCompareFoe_36);
                Player.Mana = (short)(Player.Mana + 5);
            }
        }
    }

    public static void DescribeStreet()
    {
        for (int i = 1; i < StreetSpots.Length; i++)
            if (World.Monsters[(int)StreetSpots[i].where] == Session.Room)
                Out.WriteLine(StreetSpots[i].text);
    }

    public static void GainLoot1000(short threshold, string line1, string line2, TBagItem item)
    {
        Combat.Roll = Roll(1000);
        if (Combat.Roll <= threshold)
        {
            Out.WriteLine(line1);
            if (line2 != "") Out.WriteLine(TxtUniqeItem + line2);
            Bag.Items[(int)item] = (short)(Bag.Items[(int)item] - ItemUnit);
            Bag.Count = (short)(Bag.Count + 1);
        }
    }

    public static void GainLoot100(short threshold, string line1, string line2, TBagItem item, TGainBonus bonus)
    {
        Combat.Roll = Roll(100);
        if ((Combat.Roll < threshold) && (Session.Room != DeadRoom))
        {
            Out.WriteLine(TxtShared_08 + line1);
            if (line2 != "") Out.WriteLine(TxtBang + TxtUniqeItem + line2);
            Bag.Items[(int)item] = (short)(Bag.Items[(int)item] - ItemUnit);
            switch (bonus)
            {
                case TGainBonus.gbMana: Player.MaxMana = (short)(Player.MaxMana + 40); break;
                case TGainBonus.gbCassette:
                    Player.MaxEnergy = (short)(Player.MaxEnergy + 5);
                    Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] - 8);
                    Player.Stats[(int)TStat.Dexterity] = (short)(Player.Stats[(int)TStat.Dexterity] + 1);
                    break;
            }
            Bag.Count = (short)(Bag.Count + 1);
        }
    }

    public static void SuitGain() => GainLoot1000(25, TxtSuitGain_01, TxtSuitGain_02, TBagItem.SpikedSuit);
    public static void PillGain() => GainLoot1000(42, TxtPillGain_01, "", TBagItem.TeleportPill);
    public static void CassetteGain() => GainLoot100(2, TxtCassetteGain_01, TxtCassetteGain_02, TBagItem.Cassette, TGainBonus.gbCassette);
    public static void LeafGain() => GainLoot100(6, TxtLeafGain_01, TxtLeafGain_02, TBagItem.LuckyLeaf, TGainBonus.gbMana);
    public static void ScrollGain() => GainLoot100(10, TxtScrollGain_01, "", TBagItem.CompareScroll, TGainBonus.gbNone);

    public static void EnterTrance()
    {
        Session.OuterRoom = Session.Room;
        Session.Room = TranceRoom;
    }

    public static void DescribeStage()
    {
        for (int i = 1; i < StageSpots.Length; i++)
            if (World.Monsters[(int)StageSpots[i].where] == Session.Room)
                Out.WriteLine(StageSpots[i].text);
    }

    public static void DescribeCrowd()
    {
        for (int i = 1; i < CrowdSpots.Length; i++)
            if (World.Monsters[(int)CrowdSpots[i].where] == Session.Room)
                Out.WriteLine(CrowdSpots[i].text);
    }

    public static void DescribeBeasts()
    {
        for (int i = 1; i < BeastSpots.Length; i++)
            if (World.Monsters[(int)BeastSpots[i].where] == Session.Room)
                Out.WriteLine(BeastSpots[i].text);
    }

    public static void ShieldBlock()
    {
        Combat.Roll = Roll(100);
        if (Combat.Roll <= Gear.Values[(int)TGearItem.Armor])
        {
            Combat.Damage = (short)(Combat.Damage - Gear.Values[(int)TGearItem.WornCount]);
            if (Combat.Damage < 0) Combat.Damage = 0;
            Out.WriteLine(TxtShieldBlock_01 + Combat.Damage + TxtShared_20);
        }
    }

    public static void DoFight()
    {
        for (int i = 1; i < FightExpStats.Length; i++)
        {
            TFightExpStat s = FightExpStats[i];
            Combat.FightExperience = (short)(Combat.FightExperience + s.expFor((short)(s.playerStat.Get() - s.enemyStat.Get())));
        }

        do
        {
            Combat.EnemyDodged = 0;
            Combat.PlayerDodged = 0;
            Out.WriteLine(TxtFightEngagedEnemy + Combat.EnemyEnergy + TxtFightEngagedYou + Player.Energy + TxtFightEngagedEnd);
            if (Player.Stats[(int)TStat.Dexterity] > Combat.EnemyDexterity)
            {
                Combat.DodgeChance = (short)(Player.Stats[(int)TStat.Dexterity] - Combat.EnemyDexterity);
                Combat.Roll = Roll(DodgeBound(Combat.DodgeChance));
                if (Combat.Roll < 10)
                {
                    Out.WriteLine(TxtFightYouDodgeMocking);
                    Combat.PlayerDodged = 1;
                }
                Combat.Roll = Roll(EnemyDodgeBound(Combat.DodgeChance));
                if (Combat.Roll < 10)
                {
                    Out.WriteLine(TxtEnemy + TxtFightEnemyDodgesLamersAttack);
                    Combat.EnemyDodged = 1;
                }
            }
            if (Combat.EnemyDexterity > Player.Stats[(int)TStat.Dexterity])
            {
                Combat.DodgeChance = (short)(Combat.EnemyDexterity - Player.Stats[(int)TStat.Dexterity]);
                Combat.Roll = Roll(DodgeBound(Combat.DodgeChance));
                if (Combat.Roll < 10)
                {
                    Out.WriteLine(TxtEnemy + TxtFightEnemyDodgesWithGrace);
                    Combat.EnemyDodged = 1;
                }
                Combat.Roll = Roll(EnemyDodgeBound(Combat.DodgeChance));
                if (Combat.Roll < 10)
                {
                    Out.WriteLine(TxtFightYouDodgeWithoutPardon);
                    Combat.PlayerDodged = 1;
                }
            }
            if (Combat.PlayerDodged == 0)
            {
                Combat.Damage = Roll(Combat.EnemyStrength);
                switch (Combat.Damage)
                {
                    case >= 0 and <= 5:
                        Out.WriteLine(TxtEnemy + TxtFightScratchedLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                    case >= 6 and <= 20:
                        Out.WriteLine(TxtFightTakenLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                    case >= 21 and <= 50:
                        Out.WriteLine(TxtEnemy + TxtFightMassacredYouLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                    default:
                        Out.WriteLine(TxtEnemy + TxtFightGougedYouLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                }
                Combat.FightExperience = (short)(Combat.FightExperience + 1);
                ShieldBlock();
                if ((Skills.Values[(int)TSkill.Parry] > 0) && (Combat.Damage > 0))
                {
                    Combat.Roll = Roll(140);
                    if (Combat.Roll <= Skills.Values[(int)TSkill.Parry])
                    {
                        switch (Combat.Damage)
                        {
                            case >= 0 and <= 2:
                                Out.WriteLine(TxtFightParryLabel + TxtFightParryZeroTail);
                                Combat.Damage = 0;
                                break;
                            case >= 3 and <= 9:
                                Combat.Damage = (short)(Combat.Damage - 2);
                                Out.WriteLine(TxtFightParryLabel + Combat.Damage + TxtFightTailEnergi);
                                break;
                            default:
                                Combat.Damage = (short)(Combat.Damage - 3);
                                Out.WriteLine(TxtFightParryLabel + Combat.Damage + TxtFightTailEnergi);
                                break;
                        }
                        if ((Combat.Roll < 1) && (Skills.Values[(int)TSkill.Parry] < 100))
                        {
                            Out.WriteLine(TxtFightStars15 + TxtLearnVerb + TxtFightParryLearn);
                            Skills.Values[(int)TSkill.Parry]++;
                            Combat.FightExperience = (short)(Combat.FightExperience + 5);
                        }
                    }
                }
                Player.Energy = (short)(Player.Energy - Combat.Damage);
                if (Combat.FireballCharges > 0)
                {
                    Combat.Roll = Roll(100);
                    if (Combat.Roll < 10)
                    {
                        Crt.TextColor(14);
                        Combat.Roll = Roll(20);
                        Out.WriteLine(TxtEnemy + TxtFightFireballLabel + Combat.Roll + TxtFightTailEnergii);
                        Player.Energy = (short)(Player.Energy - Combat.Roll);
                        Combat.FireballCharges = (sbyte)(Combat.FireballCharges - 1);
                        Crt.TextColor(15);
                    }
                }
                if (Combat.PoisonCharges > 0)
                {
                    Combat.Roll = Roll(100);
                    if (Combat.Roll < 10)
                    {
                        Crt.TextColor(4);
                        Out.WriteLine(TxtEnemy + TxtFightCastsPoison);
                        Combat.PoisonCharges = (sbyte)(Combat.PoisonCharges - 1);
                        Crt.TextColor(15);
                        Combat.PoisonRounds = (sbyte)(Combat.PoisonRounds + 10);
                    }
                }
                if (Combat.PoisonRounds > 0)
                {
                    Combat.Roll = Roll(5);
                    Out.WriteLine(TxtFightPoisonedLabel + Combat.Roll + TxtFightTailEnergi);
                    Player.Energy = (short)(Player.Energy - Combat.Roll);
                    Combat.PoisonRounds = (sbyte)(Combat.PoisonRounds - 1);
                }
            }
            if (Combat.EnemyDodged == 0)
            {
                Combat.Damage = Roll(Player.Stats[(int)TStat.Strength]);
                if (Combat.Damage * 10 < Gear.Values[(int)TGearItem.LuckRoll])
                {
                    do
                    {
                        Combat.Damage = Roll(Player.Stats[(int)TStat.Strength]);
                        Out.WriteLine(TxtFightLuckRoll);
                    } while (!((Combat.Damage * 10) > Gear.Values[(int)TGearItem.LuckRoll]));
                }
                Combat.EnemyEnergy = (short)(Combat.EnemyEnergy - Combat.Damage);
                switch (Combat.Damage)
                {
                    case >= 0 and <= 5:
                        Out.WriteLine(TxtFightPushedLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                    case >= 6 and <= 20:
                        Out.WriteLine(TxtFightHitLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                    case >= 21 and <= 50:
                        Out.WriteLine(TxtFightMassacredHimLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                    default:
                        Out.WriteLine(TxtFightGougedHimLabel + Combat.Damage + TxtFightTailEnergii);
                        break;
                }
            }
            Crt.Delay(2000);
            if ((Skills.Values[(int)TSkill.Kick] > 0) && (Player.Mana > Skills.Values[(int)TSkill.KickMinMana]) && (Player.Energy < Skills.Values[(int)TSkill.KickMaxEnergy]))
            {
                Combat.Roll = Roll(100);
                if (Combat.Roll <= Skills.Values[(int)TSkill.Kick] - 10)
                {
                    Combat.Roll = (short)(Roll(10) + Roll(Player.Level));
                    Crt.TextColor(14);
                    Out.WriteLine(TxtFightKickVerb + TxtFightSuperKickLabel + Combat.Roll + TxtFightTailEnergi);
                    Crt.TextColor(15);
                    Player.Mana = (short)(Player.Mana - Roll(3) - 3);
                    Combat.EnemyEnergy = (short)(Combat.EnemyEnergy - Combat.Roll);
                }
                else Out.WriteLine(TxtFightKickVerb + TxtFightSuperKickMiss);
                Player.Mana = (short)(Player.Mana - Roll(2) - 2);
            }
            if ((Skills.Values[(int)TSkill.Fleeing] > 0) && (Player.Energy < Skills.Values[(int)TSkill.FleeEnergy]) && (Player.Mana > 14))
            {
                Player.Mana = (short)(Player.Mana - 15);
                Combat.Roll = Roll(100);
                if (Combat.Roll <= Skills.Values[(int)TSkill.Fleeing])
                {
                    Combat.Fled = 1;
                    Out.WriteLine(TxtFightFled);
                    Player.Experience = (short)(Player.Experience - 20);
                }
                else Out.WriteLine(TxtFightFleeFailed);
            }
        } while (!((Player.Energy < 1) || (Combat.EnemyEnergy < 1) || (Combat.Fled == 1)));
        if ((Combat.EnemyEnergy < 1) && (Player.Energy > 0))
        {
            if (Player.MaxEnergy > 75) Combat.FightExperience = (short)(Combat.FightExperience - 2);
            if (Player.MaxEnergy > 115) Combat.FightExperience = (short)(Combat.FightExperience - 3);
            if (Skills.Values[(int)TSkill.Parry] > 50) Combat.FightExperience = (short)(Combat.FightExperience - 2);
            if (Skills.Values[(int)TSkill.Parry] > 75) Combat.FightExperience = (short)(Combat.FightExperience - 2);
            if (Skills.Values[(int)TSkill.Parry] > 95) Combat.FightExperience = (short)(Combat.FightExperience - 1);
            if (Skills.Values[(int)TSkill.Kick] > 50) Combat.FightExperience = (short)(Combat.FightExperience - 5);
            if (Skills.Values[(int)TSkill.Kick] > 95) Combat.FightExperience = (short)(Combat.FightExperience - 2);
            if (Combat.FightExperience < 0) Combat.FightExperience = 0;
            Out.WriteLine(TxtFightVictoryLabel + Combat.FightExperience + TxtFightVictoryTail);
            Player.Experience = (short)(Combat.FightExperience + Player.Experience);
            Combat.FireballCharges = 0;
            Combat.PoisonCharges = 0;

            if (World.Quest > 0) World.QuestKills = (short)(World.QuestKills - 1);
            if (Skills.Values[(int)TSkill.Cooking] > 0)
            {
                Combat.Roll = Roll(100);
                if (Skills.Values[(int)TSkill.Cooking] > Combat.Roll)
                {
                    Out.WriteLine(TxtFightMadeBigos);
                    Bag.Items[(int)TBagItem.Bigos] = (short)(Bag.Items[(int)TBagItem.Bigos] - ItemUnit);
                    Bag.Count = (short)(Bag.Count + 1);
                    if ((Combat.Roll < 1) && (Skills.Values[(int)TSkill.Cooking] < 100))
                    {
                        Out.WriteLine(TxtFightCookingLearn);
                        Skills.Values[(int)TSkill.Cooking]++;
                        Player.Experience = (short)(Player.Experience + 10);
                    }
                }
            }
        }
        if (Player.Energy < 1)
        {
            Out.WriteLine(TxtFightKilled);
            Crt.Delay(3000);
            Session.Room = DeadRoom;
        }
        Combat.FightExperience = 0;
    }

    public sealed class TFightExpStat
    {
        public IntSlot playerStat = null!, enemyStat = null!;
        public Func<short, short> expFor = _ => 0;
    }

    public static readonly TFightExpStat[] FightExpStats = { null!,
        new() { playerStat = new IntSlot(() => Player.MaxEnergy, v => Player.MaxEnergy = v), enemyStat = new IntSlot(() => Combat.EnemyEnergy, v => Combat.EnemyEnergy = v), expFor = FightExpForEnergy },
        new() { playerStat = IntSlot.Into(Player.Stats, (int)TStat.Strength), enemyStat = new IntSlot(() => Combat.EnemyStrength, v => Combat.EnemyStrength = v), expFor = FightExpForStrength },
        new() { playerStat = IntSlot.Into(Player.Stats, (int)TStat.Dexterity), enemyStat = new IntSlot(() => Combat.EnemyDexterity, v => Combat.EnemyDexterity = v), expFor = FightExpForDexterity },
    };

    public static void TakeItem(string cmd, TBagItem item, string verb, string line, TTakeEffect effect)
    {
        if ((Command == cmd) && (Bag.Items[(int)item] == Session.OuterRoom))
        {
            Out.WriteLine(verb + line);
            Bag.Items[(int)item] = -ItemUnit;
            Bag.Count = (short)(Bag.Count + 1);
            switch (effect)
            {
                case TTakeEffect.teDiploma: Player.MaxEnergy = (short)(Player.MaxEnergy + 5); break;
                case TTakeEffect.tePipe:
                    if (Player.Stats[(int)TStat.Wisdom] < Player.Stats[(int)TStat.MaxWisdom])
                        Player.Stats[(int)TStat.Wisdom]++;
                    break;
            }
        }
    }

    public static void DropItem(string cmd, TBagItem item, string verb, string line, TTakeEffect effect)
    {
        if ((Command == cmd) && (Bag.Items[(int)item] == -ItemUnit))
        {
            Out.WriteLine(verb + line);
            Bag.Items[(int)item] = Session.OuterRoom;
            Bag.Count = (short)(Bag.Count - 1);
            switch (effect)
            {
                case TTakeEffect.teDiploma: Player.MaxEnergy = (short)(Player.MaxEnergy - 5); break;
                case TTakeEffect.tePipe: Player.Stats[(int)TStat.Wisdom]--; break;
            }
        }
    }

    public sealed class TTakeDrop
    {
        public string cmd = "", verb = "", line = "";
        public TBagItem item;
        public TTakeEffect effect;
    }

    public static readonly TTakeDrop[] TakeItems = { null!,
        new() { cmd = CmdBierz + CmdStary, item = TBagItem.OldSword, verb = TxtPickUpVerb, line = TxtTakeDrop_01, effect = TTakeEffect.teNone },
        new() { cmd = CmdBierz + CmdMala, item = TBagItem.SmallShield, verb = TxtPickUpVerb, line = TxtTakeDrop_02, effect = TTakeEffect.teNone },
        new() { cmd = CmdBierz + CmdSerce, item = TBagItem.Heart, verb = TxtPickUpVerb, line = TxtTakeDrop_03, effect = TTakeEffect.teNone },
        new() { cmd = CmdBierz + CmdDyplom, item = TBagItem.Diploma, verb = TxtPickUpVerb, line = TxtTakeDrop_04, effect = TTakeEffect.teDiploma },
        new() { cmd = CmdBierz + CmdFajka, item = TBagItem.Pipe, verb = TxtPickUpVerb, line = TxtTakeDrop_05, effect = TTakeEffect.tePipe },
    };

    public static readonly TTakeDrop[] DropItems = { null!,
        new() { cmd = CmdOdrzuc + CmdStary, item = TBagItem.OldSword, verb = TxtDropVerb, line = TxtTakeDrop_06, effect = TTakeEffect.teNone },
        new() { cmd = CmdOdrzuc + CmdFajka, item = TBagItem.Pipe, verb = "", line = TxtTakeDrop_07, effect = TTakeEffect.tePipe },
        new() { cmd = CmdOdrzuc + CmdMala, item = TBagItem.SmallShield, verb = TxtDropVerb, line = TxtTakeDrop_08, effect = TTakeEffect.teNone },
        new() { cmd = CmdOdrzuc + CmdSerce, item = TBagItem.Heart, verb = TxtDropVerb, line = TxtTakeDrop_09, effect = TTakeEffect.teNone },
        new() { cmd = CmdOdrzuc + CmdDyplom, item = TBagItem.Diploma, verb = TxtDropVerb, line = TxtTakeDrop_10, effect = TTakeEffect.teDiploma },
    };

    public static void TakeDrop()
    {
        for (int ti = 1; ti < TakeItems.Length; ti++)
        {
            TTakeDrop t = TakeItems[ti];
            TakeItem(t.cmd, t.item, t.verb, t.line, t.effect);
        }
        for (int ti = 1; ti < DropItems.Length; ti++)
        {
            TTakeDrop t = DropItems[ti];
            DropItem(t.cmd, t.item, t.verb, t.line, t.effect);
        }
    }

    public sealed class TFoodItem
    {
        public string cmd = "", verb = "", line = "";
        public TBagItem item;
        public short gain;
        public IntSlot stat = null!, cap = null!;
    }

    private static IntSlot PlayerEnergy() => new(() => Player.Energy, v => Player.Energy = v);
    private static IntSlot PlayerMaxEnergy() => new(() => Player.MaxEnergy, v => Player.MaxEnergy = v);
    private static IntSlot PlayerMana() => new(() => Player.Mana, v => Player.Mana = v);
    private static IntSlot PlayerMaxMana() => new(() => Player.MaxMana, v => Player.MaxMana = v);

    public static readonly TFoodItem[] FoodItems = { null!,
        new() { cmd = CmdUzyj + CmdPaczek, item = TBagItem.Doughnut, verb = TxtEatVerb, line = TxtUseItems_12, gain = 8, stat = PlayerEnergy(), cap = PlayerMaxEnergy() },
        new() { cmd = CmdUzyj + CmdCiastko, item = TBagItem.Cookie, verb = TxtEatVerb, line = TxtUseItems_13, gain = 12, stat = PlayerEnergy(), cap = PlayerMaxEnergy() },
        new() { cmd = CmdUzyj + CmdSuchaRacja, item = TBagItem.DryRation, verb = TxtEatVerb, line = TxtUseItems_14, gain = 16, stat = PlayerEnergy(), cap = PlayerMaxEnergy() },
        new() { cmd = CmdUzyj + CmdBulka, item = TBagItem.BreadRoll, verb = TxtEatVerb, line = TxtUseItems_15, gain = 20, stat = PlayerEnergy(), cap = PlayerMaxEnergy() },
        new() { cmd = CmdUzyj + CmdChleb, item = TBagItem.Bread, verb = TxtEatVerb, line = TxtUseItems_23, gain = 26, stat = PlayerEnergy(), cap = PlayerMaxEnergy() },
        new() { cmd = CmdUzyj + CmdWeka, item = TBagItem.Sausage, verb = TxtEatVerb, line = TxtUseItems_25, gain = 34, stat = PlayerEnergy(), cap = PlayerMaxEnergy() },
        new() { cmd = CmdUzyj + CmdMalaButelkaMany, item = TBagItem.ManaBottle, verb = "", line = TxtUseItems_26, gain = 30, stat = PlayerMana(), cap = PlayerMaxMana() },
        new() { cmd = CmdUzyj + CmdBigos, item = TBagItem.Bigos, verb = TxtEatVerb, line = TxtUseItems_27, gain = 20, stat = PlayerEnergy(), cap = PlayerMaxEnergy() },
    };

    public static void ClampStat(ref short stat, short cap)
    {
        if (stat > cap) stat = cap;
    }

    public static void EatFood(string cmd, TBagItem item, string verb, string line, short gain, IntSlot stat, IntSlot cap)
    {
        if ((Command == cmd) && (Bag.Items[(int)item] <= -ItemUnit))
        {
            Bag.Count = (short)(Bag.Count - 1);
            Bag.Items[(int)item] = (short)(Bag.Items[(int)item] + ItemUnit);
            Out.WriteLine(verb + line);
            stat.Set((short)(stat.Get() + gain));
            short tmp = stat.Get();
            ClampStat(ref tmp, cap.Get());
            stat.Set(tmp);
        }
    }

    public static void UseItems()
    {
        if ((Command == CmdUzyj + CmdFajka) && (Bag.Items[(int)TBagItem.Pipe] == -ItemUnit))
            Out.WriteLine(TxtUseItems_01);
        if ((Command == CmdUzyj + CmdSerce) && (Bag.Items[(int)TBagItem.Heart] == -ItemUnit))
        {
            Bag.Items[(int)TBagItem.Heart] = 0;
            Out.WriteLine(TxtEatVerb + TxtUseItems_02);
            Player.Energy = (short)(Player.Energy + 5);
            short tmpEnergy = Player.Energy;
            ClampStat(ref tmpEnergy, Player.MaxEnergy);
            Player.Energy = tmpEnergy;
            Bag.Count = (short)(Bag.Count - 1);
        }
        if ((Command == CmdUzyj + CmdDyplom) && (Bag.Items[(int)TBagItem.Diploma] == -ItemUnit))
            WriteLines(TxtUseItemsLines1[1..]);
        for (int fi = 1; fi < FoodItems.Length; fi++)
        {
            TFoodItem f = FoodItems[fi];
            EatFood(f.cmd, f.item, f.verb, f.line, f.gain, f.stat, f.cap);
        }
        if ((Command == CmdZniszcz + CmdPrzepustka) && (Bag.Permit <= -ItemUnit))
        {
            Out.WriteLine(TxtUseItems_16);
            Bag.Permit = Bag.Permit + ItemUnit;
            Bag.Count = (short)(Bag.Count + 1);
        }
        if ((Command == CmdPatrz + CmdPrzepustka) && (Bag.Permit <= -ItemUnit))
            WriteLines(TxtUseItemsLines2[1..]);
        if (Command == CmdUzyj + CmdKaseta)
            Out.WriteLine(TxtUseItems_24);
        if ((Command == CmdUzyj + CmdKomplet) && (Bag.Items[(int)TBagItem.Tracksuit] <= -ItemUnit) && (Gear.WornGarment == TxtEmpty))
        {
            Out.WriteLine(TxtUseItems_28);
            Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] + 7);
            Gear.Values[(int)TGearItem.Wearing] = 1;
            Gear.WornGarment = TxtWornSyf;
        }
        if ((Command == CmdOdlorz + CmdKomplet) && (Bag.Items[(int)TBagItem.Tracksuit] <= -ItemUnit) && (Gear.WornGarment == TxtWornSyf))
        {
            Out.WriteLine(TxtUseItems_29);
            Gear.Values[(int)TGearItem.Wearing] = 0;
            Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] - 7);
            Gear.WornGarment = TxtEmpty;
        }
        if ((Command == CmdUzyj + CmdListek) && (Bag.Items[(int)TBagItem.LuckyLeaf] <= -ItemUnit))
            Out.WriteLine(TxtUseItems_30);
        if ((Command == CmdUzyj + TxtWornGarnitur) && (Bag.Items[(int)TBagItem.SpikedSuit] <= -ItemUnit) && (Gear.WornGarment == TxtEmpty))
        {
            Out.WriteLine(TxtUseItems_31);
            Gear.Values[(int)TGearItem.Wearing] = 1;
            Gear.Values[(int)TGearItem.WornCount] = (short)(Gear.Values[(int)TGearItem.WornCount] + 1);
            Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] + 10);
            Gear.Values[(int)TGearItem.LuckRoll] = (short)(Gear.Values[(int)TGearItem.LuckRoll] + 15);
            Gear.WornGarment = TxtWornGarnitur;
        }
        if ((Command == CmdUzyj + CmdPigulka) && (Bag.Items[(int)TBagItem.TeleportPill] <= -ItemUnit))
        {
            Out.WriteLine(TxtUseItems_32);
            Bag.Items[(int)TBagItem.TeleportPill] = (short)(Bag.Items[(int)TBagItem.TeleportPill] + ItemUnit);
            SpawnMonsters();
            if (Player.Stats[(int)TStat.Wisdom] < 10)
            {
                Out.WriteLine(TxtUseItems_33);
                Out.WriteLine(TxtUseItems_34);
                Player.MaxEnergy = (short)(Player.MaxEnergy - 1);
                Player.Energy = 1;
                Player.Experience = (short)(Player.Experience - 50);
            }
            else if (Player.Stats[(int)TStat.Wisdom] < 16)
            {
                Out.WriteLine(TxtUseItems_35);
                Out.WriteLine(TxtUseItems_36);
                Player.Energy = (short)(Player.Energy - 40);
                if (Player.Energy < 1) Player.Energy = 1;
                Player.Experience = (short)(Player.Experience - 30);
            }
            else
                Out.WriteLine(TxtUseItems_37);
        }
        if ((Command == CmdOdlorz + TxtWornGarnitur) && (Bag.Items[(int)TBagItem.SpikedSuit] <= -ItemUnit) && (Gear.WornGarment == TxtWornGarnitur))
        {
            Out.WriteLine(TxtUseItems_38);
            Gear.Values[(int)TGearItem.Wearing] = 0;
            Gear.Values[(int)TGearItem.WornCount] = (short)(Gear.Values[(int)TGearItem.WornCount] - 1);
            Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] - 10);
            Gear.Values[(int)TGearItem.LuckRoll] = (short)(Gear.Values[(int)TGearItem.LuckRoll] - 15);
            Gear.WornGarment = TxtEmpty;
        }
    }

    public static void DisplayPrefs()
    {
        if (Command == CmdZmien + CmdKolor)
        {
            Out.WriteLine(TxtDisplayPrefs_01);
            Combat.Roll = Crt.ReadShort();
            Crt.TextColor(Combat.Roll);
        }
        if (Command == CmdZmien + CmdTlo)
        {
            Out.WriteLine(TxtDisplayPrefs_02);
            Combat.Roll = Crt.ReadShort();
            Crt.TextBackground(Combat.Roll);
        }
    }

    public sealed class TBeastKill
    {
        public string cmd = "";
        public TMonster beast;
        public TFoe foe;
        public bool unlessFled;
    }

    public static readonly TBeastKill[] BeastKills = { null!,
        new() { cmd = CmdZabij + CmdSlimak, beast = TMonster.Snail, foe = TFoe.WeakEnemy, unlessFled = true },
        new() { cmd = CmdZabij + CmdKornik, beast = TMonster.Woodworm, foe = TFoe.WeakEnemy, unlessFled = true },
        new() { cmd = CmdZabij + CmdMucha, beast = TMonster.Fly, foe = TFoe.WeakEnemy, unlessFled = true },
        new() { cmd = CmdZabij + CmdZuk, beast = TMonster.Beetle, foe = TFoe.WeakEnemy, unlessFled = true },
        new() { cmd = CmdZabij + CmdKaraluch, beast = TMonster.Cockroach, foe = TFoe.WeakEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdMrowka, beast = TMonster.Ant, foe = TFoe.WeakEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdPajak, beast = TMonster.Spider, foe = TFoe.WeakEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdDzik, beast = TMonster.Boar, foe = TFoe.WeakishEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdSzczur, beast = TMonster.Rat, foe = TFoe.WeakishEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdLis, beast = TMonster.Fox, foe = TFoe.WeakishEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdKuropatwa, beast = TMonster.Partridge, foe = TFoe.WeakishEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdZajac, beast = TMonster.Hare, foe = TFoe.WeakishEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdOrzel, beast = TMonster.Eagle, foe = TFoe.WeakishEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdSarna, beast = TMonster.Deer, foe = TFoe.WeakishEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdSlon, beast = TMonster.Elephant, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdLew, beast = TMonster.Lion, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdZyrafa, beast = TMonster.Giraffe, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdWilk, beast = TMonster.Wolf, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdWielblad, beast = TMonster.Camel, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdStrus, beast = TMonster.Ostrich, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdBoa, beast = TMonster.Boa, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdBizon, beast = TMonster.Bison, foe = TFoe.MediumEnemy, unlessFled = false },
        new() { cmd = CmdZabij + CmdPantera, beast = TMonster.Panther, foe = TFoe.MediumEnemy, unlessFled = false },
    };

    public sealed class TArenaKill
    {
        public string cmd = "";
        public TMonster monster;
        public bool pills;
    }

    public static readonly TArenaKill[] ArenaKills = { null!,
        new() { cmd = CmdZabij + CmdGladiator, monster = TMonster.Gladiator, pills = true },
        new() { cmd = CmdZabij + CmdWojownik, monster = TMonster.Warrior, pills = true },
        new() { cmd = CmdZabij + CmdTrener, monster = TMonster.Tamer, pills = false },
    };

    public static void ArenaKill(string cmd, TMonster monster, bool pills)
    {
        if ((Command == cmd) && (Session.Room == World.Monsters[(int)monster]))
        {
            Fight(TFoe.HardEnemy);
            World.Monsters[(int)monster] = 0;
            if (pills) PillGain(); else SuitGain();
        }
    }

    public sealed class TMazeMove
    {
        public short room;
        public short[] dest = new short[4];
    }

    public static readonly TMazeMove[] MazeMoves = { null!,
        new() { room = RoomArena33, dest = new short[] { RoomArenaEntrance, RoomArena34, RoomArena35, RoomArena36 } },
        new() { room = RoomArena34, dest = new short[] { RoomArena33, RoomArena37, RoomArena38, RoomArena39 } },
        new() { room = RoomArena35, dest = new short[] { RoomArena35, RoomArena38, RoomArena40, RoomArena33 } },
        new() { room = RoomArena36, dest = new short[] { RoomArena36, RoomArena39, RoomArena33, RoomArena41 } },
        new() { room = RoomArena37, dest = new short[] { RoomArena34, RoomArena42, RoomArena43, RoomArena44 } },
        new() { room = RoomArena38, dest = new short[] { RoomArena35, RoomArena43, RoomArena45, RoomArena34 } },
        new() { room = RoomArena39, dest = new short[] { RoomArena36, RoomArena44, RoomArena34, RoomArena46 } },
        new() { room = RoomArena40, dest = new short[] { RoomArena40, RoomArena45, RoomArena40, RoomArena35 } },
        new() { room = RoomArena41, dest = new short[] { RoomArena41, RoomArena46, RoomArena36, RoomArena41 } },
        new() { room = RoomArena42, dest = new short[] { RoomArena37, RoomArena47, RoomArena48, RoomArena49 } },
        new() { room = RoomArena43, dest = new short[] { RoomArena38, RoomArena48, RoomArena50, RoomArena37 } },
        new() { room = RoomArena44, dest = new short[] { RoomArena39, RoomArena49, RoomArena37, RoomArena51 } },
        new() { room = RoomArena45, dest = new short[] { RoomArena40, RoomArena50, RoomArena45, RoomArena38 } },
        new() { room = RoomArena46, dest = new short[] { RoomArena41, RoomArena51, RoomArena39, RoomArena46 } },
        new() { room = RoomArena47, dest = new short[] { RoomArena42, RoomArena47, RoomArena52, RoomArena53 } },
        new() { room = RoomArena48, dest = new short[] { RoomArena43, RoomArena52, RoomArena54, RoomArena42 } },
        new() { room = RoomArena49, dest = new short[] { RoomArena44, RoomArena53, RoomArena42, RoomArena55 } },
        new() { room = RoomArena50, dest = new short[] { RoomArena45, RoomArena54, RoomArena50, RoomArena43 } },
        new() { room = RoomArena51, dest = new short[] { RoomArena46, RoomArena55, RoomArena44, RoomArena51 } },
        new() { room = RoomArena52, dest = new short[] { RoomArena48, RoomArena52, RoomArena56, RoomArena47 } },
        new() { room = RoomArena53, dest = new short[] { RoomArena49, RoomArena53, RoomArena47, RoomArena57 } },
        new() { room = RoomArena54, dest = new short[] { RoomArena50, RoomArena56, RoomArena54, RoomArena48 } },
        new() { room = RoomArena55, dest = new short[] { RoomArena51, RoomArena57, RoomArena49, RoomArena55 } },
        new() { room = RoomArena56, dest = new short[] { RoomArena54, RoomArena56, RoomArena56, RoomArena52 } },
        new() { room = RoomArena57, dest = new short[] { RoomArena55, RoomArena57, RoomArena53, RoomArena57 } },
    };

    public static void MazeMove(TMazeDir dir)
    {
        Session.InArena = 0;
        for (int mi = 1; mi < MazeMoves.Length; mi++)
        {
            TMazeMove m = MazeMoves[mi];
            if (Session.Room == m.room)
            {
                if (m.dest[(int)dir] == m.room) Session.InArena = 1;
                else Session.Room = m.dest[(int)dir];
                break;
            }
        }
    }

    public static void ArenaMaze()
    {
        if ((Session.Room >= RoomArena33) && (Session.Room <= RoomArena57))
        {
            Session.InArena = 1;
            Out.WriteLine(TxtArenaMaze_01);
            DescribeBeasts();
            do
            {
                Out.Write(Player.Energy + TxtShared_01 + Player.Experience + TxtShared_02);
                Command = Crt.ReadLn();
                ExpandAbbrev();
                CheckMode();
                if ((Command == CmdZabij + CmdBakteria) && (Session.Room == World.Monsters[(int)TMonster.Bacterium]))
                {
                    Fight(TFoe.WeakEnemy);
                    World.Monsters[(int)TMonster.Bacterium] = 0;
                    Out.WriteLine(TxtArenaMaze_02);
                }
                for (int bi = 1; bi < BeastKills.Length; bi++)
                {
                    TBeastKill b = BeastKills[bi];
                    if (b.unlessFled)
                        KillBeastUnlessFled(b.cmd, ref World.Monsters[(int)b.beast], b.foe);
                    else
                        KillBeast(b.cmd, ref World.Monsters[(int)b.beast], b.foe);
                }
                if ((Command == CmdZabij + CmdWilczur) && (Session.Room == World.Monsters[(int)TMonster.Mongrel]))
                {
                    Out.WriteLine(TxtArenaMaze_03);
                    Fight(TFoe.WeakishEnemy);
                    World.Monsters[(int)TMonster.Mongrel] = 0;
                }

                for (int ai = 1; ai < ArenaKills.Length; ai++)
                {
                    TArenaKill a = ArenaKills[ai];
                    ArenaKill(a.cmd, a.monster, a.pills);
                }
                if (Command == CmdExit)
                    switch (Session.Room)
                    {
                        case RoomArena33:
                            WriteExits(new[] { TxtShared_03,
                                TxtExitPolodnie, TxtPlaceWejscieNaArene,
                                TxtExitPolnoc, TxtPlaceArena,
                                TxtExitWschod, TxtPlaceArena,
                                TxtExitZachod, TxtPlaceArena });
                            break;
                        case RoomArena34:
                        case RoomArena37:
                        case RoomArena38:
                        case RoomArena39:
                        case RoomArena42:
                        case RoomArena43:
                        case RoomArena44:
                        case RoomArena48:
                        case RoomArena49:
                            WriteExits(new[] { TxtShared_03,
                                TxtExitPolodnie, TxtPlaceArena,
                                TxtExitPolnoc, TxtPlaceArena,
                                TxtExitWschod, TxtPlaceArena,
                                TxtExitZachod, TxtPlaceArena });
                            break;
                        case RoomArena35:
                        case RoomArena36:
                            WriteExits(new[] { TxtShared_03,
                                TxtExitPolnoc, TxtPlaceArena,
                                TxtExitWschod, TxtPlaceArena,
                                TxtExitZachod, TxtPlaceArena });
                            break;
                        case RoomArena40:
                            WriteExits(new[] { TxtShared_03,
                                TxtExitPolnoc, TxtPlaceArena,
                                TxtExitZachod, TxtPlaceArena });
                            break;
                        case RoomArena41:
                            WriteExits(new[] { TxtShared_03,
                                TxtExitPolnoc, TxtPlaceArena,
                                TxtExitWschod, TxtPlaceArena });
                            break;
                        case RoomArena45:
                        case RoomArena50:
                        case RoomArena54:
                            WriteExits(new[] { TxtArenaMaze_08,
                                TxtExitPolnoc, TxtPlaceArena,
                                TxtExitPolodnie, TxtPlaceArena,
                                TxtExitZachod, TxtPlaceArena });
                            break;
                        case RoomArena46:
                        case RoomArena51:
                        case RoomArena55:
                            WriteExits(new[] { TxtArenaMaze_08,
                                TxtExitPolnoc, TxtPlaceArena,
                                TxtExitPolodnie, TxtPlaceArena,
                                TxtExitWschod, TxtPlaceArena });
                            break;
                        case RoomArena47:
                        case RoomArena52:
                        case RoomArena53:
                            WriteExits(new[] { TxtArenaMaze_09,
                                TxtExitPolodnie, TxtPlaceArena,
                                TxtExitWschod, TxtPlaceArena,
                                TxtExitZachod, TxtPlaceArena });
                            break;
                        case RoomArena56:
                            WriteExits(new[] { TxtArenaMaze_10,
                                TxtExitPolodnie, TxtPlaceArena,
                                TxtExitZachod, TxtPlaceArena });
                            break;
                        case RoomArena57:
                            WriteExits(new[] { TxtArenaMaze_10,
                                TxtExitPolodnie, TxtPlaceArena,
                                TxtExitWschod, TxtPlaceArena });
                            break;
                    }
                if (Command == CmdWyjscie) return;
                if (Command == CmdPolodnie)
                    MazeMove(TMazeDir.mdPolodnie);
                if (Command == CmdPolnoc)
                {
                    MazeMove(TMazeDir.mdPolnoc);
                    Session.MoveGuard = 1;
                }
                if (Command == CmdWschod)
                {
                    MazeMove(TMazeDir.mdWschod);
                    Session.MoveGuard = 1;
                }
                if (Command == CmdZachod)
                {
                    MazeMove(TMazeDir.mdZachod);
                    Session.MoveGuard = 1;
                }
                Session.MoveGuard = 0;
            } while (!((Session.InArena == 0) || (Session.Room == TranceRoom)));
        }
    }

    public static void StageFights()
    {
        if ((Command == CmdZabij + CmdPerkusista) && (World.Monsters[(int)TMonster.Drummer] == Session.Room))
        {
            Fight(TFoe.VeryEasyEnemy);
            World.Monsters[(int)TMonster.Drummer] = 0;
            CassetteGain();
        }
        if ((Command == CmdZabij + CmdGitarzysta) && (World.Monsters[(int)TMonster.Guitarist] == Session.Room))
        {
            Fight(TFoe.VeryEasyEnemy);
            World.Monsters[(int)TMonster.Guitarist] = 0;
            CassetteGain();
        }
        if ((Command == CmdZabij + CmdLiroy) && (World.Monsters[(int)TMonster.Liroy] == Session.Room))
        {
            Fight(TFoe.NotEasyEnemy);
            if ((Session.Room != DeadRoom) && (Player.Energy > 0) && (Combat.Fled == 0))
            {
                Out.WriteLine(TxtStageFights_01);
                Player.Money = Player.Money + 30;
                CassetteGain();
                if (World.Quest > 1) World.QuestKills = (short)(World.QuestKills - 150);
                World.Monsters[(int)TMonster.Liroy] = 0;
            }
        }
        if ((Command == CmdZabij + CmdOrganista) && (World.Monsters[(int)TMonster.Keyboardist] == Session.Room))
        {
            Fight(TFoe.VeryEasyEnemy);
            World.Monsters[(int)TMonster.Keyboardist] = 0;
            CassetteGain();
        }
    }

    public static void KillPlant(string cmd, ref short Plant)
    {
        if ((Command == cmd) && (Plant == Session.Room))
        {
            Fight(TFoe.VeryEasyEnemy);
            Plant = 0;
        }
    }

    public static void KillPlantIfAlive(string cmd, ref short Plant)
    {
        if ((Command == cmd) && (Plant == Session.Room))
        {
            Fight(TFoe.VeryEasyEnemy);
            if (Player.Energy > 0) Plant = 0;
        }
    }

    public static void KillPlantEasyIfAlive(string cmd, ref short Plant)
    {
        if ((Command == cmd) && (Plant == Session.Room))
        {
            Fight(TFoe.EasyEnemy);
            if (Player.Energy > 0) Plant = 0;
        }
    }

    public sealed class TPlantKill
    {
        public string cmd = "";
        public TMonster plant;
    }

    public static readonly TPlantKill[] PlantKills = { null!,
        new() { cmd = CmdZabij + CmdSzczaw, plant = TMonster.Sorrel },
        new() { cmd = CmdZabij + CmdKoniczynka, plant = TMonster.Clover },
        new() { cmd = CmdZabij + CmdMlecz, plant = TMonster.SowThistle },
        new() { cmd = CmdZabij + CmdDmuchawiec, plant = TMonster.Dandelion },
    };

    public static readonly TPlantKill[] EasyPlantKills = { null!,
        new() { cmd = CmdZabij + CmdRoza, plant = TMonster.Rose },
        new() { cmd = CmdZabij + CmdOset, plant = TMonster.Thistle },
        new() { cmd = CmdZabij + CmdMalina, plant = TMonster.Raspberry },
        new() { cmd = CmdZabij + CmdAgrest, plant = TMonster.Gooseberry },
        new() { cmd = CmdZabij + CmdJezyna, plant = TMonster.Blackberry },
    };

    public static void PlantFights()
    {
        if ((Command == CmdZabij + CmdDuncan) && (World.Monsters[(int)TMonster.Duncan] == Session.Room))
        {
            Fight(TFoe.EasyEnemy);
            if (Combat.EnemyEnergy < 1) World.Monsters[(int)TMonster.Duncan] = 0;
        }
        if ((Command == CmdSecret + CmdLista) && (World.Monsters[(int)TMonster.Duncan] == Session.Room))
            WriteLines(TxtPlantFightsLines1[1..]);
        if (Command == CmdKup + CmdDokument)
        {
            if (Player.Money > 399)
            {
                Out.WriteLine(TxtPlantFights_05);
                Bag.Permit = Bag.Permit - ItemUnit;
                Player.Money = Player.Money - 400;
                Bag.Count = (short)(Bag.Count + 1);
            }
        }
        if ((Command == CmdKup + CmdPlecak) && (World.Monsters[(int)TMonster.Duncan] == Session.Room))
        {
            if (Player.Money > 4799)
            {
                Out.WriteLine(TxtPlantFights_06);
                Player.Money = Player.Money - 4800;
                Bag.Items[(int)TBagItem.Backpack] = (short)(Bag.Items[(int)TBagItem.Backpack] - ItemUnit);
                Bag.Count = (short)(Bag.Count + 1);
            }
        }
        if ((Command == CmdRozmawiaj + CmdDuncan) && (World.Monsters[(int)TMonster.Duncan] == Session.Room))
        {
            if (World.DuncanQuest == 0)
            {
                WriteLines(TxtPlantFightsLines2[1..]);
                World.DuncanQuest = 75;
            }
            if (World.DuncanQuest != 0)
                Out.WriteLine(TxtPlantFights_13);

            if (World.DuncanQuest == -125)
            {
                Out.WriteLine(TxtPlantFights_14);
                Out.WriteLine(TxtPlantFights_15);
                World.DuncanQuest = 0;
                Player.Experience = (short)(Player.Experience + 125);
                Out.WriteLine(TxtPlantFights_16);
            }
        }

        for (int i = 1; i < PlantKills.Length; i++)
            KillPlant(PlantKills[i].cmd, ref World.Monsters[(int)PlantKills[i].plant]);

        KillPlantIfAlive(CmdZabij + CmdStokrotka, ref World.Monsters[(int)TMonster.Daisy]);

        for (int i = 1; i < EasyPlantKills.Length; i++)
            KillPlantEasyIfAlive(EasyPlantKills[i].cmd, ref World.Monsters[(int)EasyPlantKills[i].plant]);

        if ((Command == CmdZabij + CmdTrawa) && (World.Monsters[(int)TMonster.Grass] == Session.Room))
        {
            Out.WriteLine(TxtPlantFights_17);
            Fight(TFoe.HardEnemy);
            for (int i = 2; i <= 4; i++)
                if (Player.Energy > 0) Fight(TFoe.HardEnemy);
            if ((Player.Energy > 0) && (Combat.Fled == 0))
                Out.WriteLine(TxtPlantFights_18);
        }
    }

    public static void ReadCommand()
    {
        Out.Write(Player.Energy + TxtShared_01 + Player.Experience + TxtShared_02);
        Command = Crt.ReadLn();
    }

    public static void CheckMode()
    {
        if (Command == CmdMode) EnterTrance();
    }

    public static void ExpandAbbrev()
    {
        string[] AbbrevFrom = { "", CmdPn, CmdPd, CmdW, CmdZ, CmdG, CmdD, CmdE, CmdM };
        string[] AbbrevTo = { "", CmdPolnoc, CmdPolodnie, CmdWschod, CmdZachod, CmdGora, CmdDol, CmdExit, CmdMode };
        for (int i = 1; i <= 8; i++)
            if (Command == AbbrevFrom[i])
            {
                Command = AbbrevTo[i];
                return;
            }
    }

    public sealed class TRace
    {
        public string name = "";
        public short[] baseStats = new short[6];
        public short MaxManaBonus, MoneyBonus, MaxEnergyBonus, EnergyBonus;
    }

    public static readonly TRace[] Races = { null!,
        new() { name = CmdCzlowiek, baseStats = new short[] { 10, 10, 10, 15, 15, 15 },
            MaxManaBonus = 0, MoneyBonus = 0, MaxEnergyBonus = 0, EnergyBonus = 0 },
        new() { name = CmdOlbrzym, baseStats = new short[] { 13, 13, 5, 20, 20, 7 },
            MaxManaBonus = -30, MoneyBonus = 0, MaxEnergyBonus = 0, EnergyBonus = 0 },
        new() { name = CmdNimfa, baseStats = new short[] { 10, 13, 7, 15, 20, 11 },
            MaxManaBonus = 0, MoneyBonus = 0, MaxEnergyBonus = 0, EnergyBonus = 0 },
        new() { name = CmdPolElf, baseStats = new short[] { 6, 16, 8, 10, 24, 12 },
            MaxManaBonus = -20, MoneyBonus = 30, MaxEnergyBonus = -5, EnergyBonus = -5 },
        new() { name = CmdUfok, baseStats = new short[] { 9, 9, 13, 13, 14, 20 },
            MaxManaBonus = 50, MoneyBonus = 0, MaxEnergyBonus = 0, EnergyBonus = 0 },
        new() { name = CmdCzaromil, baseStats = new short[] { 6, 7, 16, 9, 11, 25 },
            MaxManaBonus = 150, MoneyBonus = 0, MaxEnergyBonus = 0, EnergyBonus = 0 },
    };

    public sealed class TDexterityLimit
    {
        public short gate, limit;
    }

    public static readonly TDexterityLimit[] DexterityItemLimits = { null!,
        new() { gate = 0, limit = 10 }, new() { gate = 11, limit = 11 },
        new() { gate = 13, limit = 12 }, new() { gate = 16, limit = 13 },
        new() { gate = 19, limit = 14 }, new() { gate = 21, limit = 15 },
        new() { gate = 25, limit = 16 },
    };

    public const short Lvl1_Threshold = 700;
    public const short Lvl2_Threshold = 725;
    public const short Lvl3_Threshold = 730;
    public const short Lvl4_Base = 735;
    public const short Lvl9_Base = 735;

    public static string LevelName(short lvl)
    {
        if ((lvl >= 4) && (lvl <= 13)) return LevelNames[lvl];
        else return TxtEmpty;
    }

    public static readonly string[] LevelNames = { "", "", "", "",
        TxtLvlName4, TxtLvlName5, TxtLvlName6, TxtLvlName7, TxtLvlName8,
        TxtLvlName9, TxtLvlName10, TxtLvlName11, TxtLvlName12, TxtLvlName13 };

    public static readonly TBand[] LvlThresholds = { null!,
        new(1, 1, Lvl1_Threshold, 0),
        new(2, 2, Lvl2_Threshold, 0),
        new(3, 3, Lvl3_Threshold, 0),
        new(4, 8, Lvl4_Base, 1),
        new(9, MaxInt, Lvl9_Base, 2),
    };

    public static short LevelThreshold(short lvl) => BandAmount(LvlThresholds[1..], lvl);

    public sealed class TSheetItem
    {
        public IntSlot item = null!;
        public string desc = "";
        public short width;
        public TSheetKind kind;
    }

    public static readonly TSheetItem[] SheetItems = { null!,
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.OldSword), desc = CmdStaryZardzewialyMiecz, width = 0, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.SmallShield), desc = CmdOkraglaMalaTarcza, width = 0, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Heart), desc = CmdZakrwawioneSerce, width = 0, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Diploma), desc = CmdDyplomMudSzkoly, width = 0, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Doughnut), desc = CmdSmacznyPolanyLikieremPaczek, width = 10, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Cookie), desc = CmdSmaczneCiastko, width = 10, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.DryRation), desc = CmdTwardaSuchaRacje, width = 10, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.BreadRoll), desc = CmdOkraglaPachnacaBulka, width = 10, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Bread), desc = CmdDuzyCieplyChleb, width = 10, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Sausage), desc = CmdDlugaISmacznaWeke, width = 10, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Bigos), desc = CmdPysznyBigosZKapustaZWroga, width = 10, kind = TSheetKind.ikShown },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Beer), desc = CmdOzewiajacePiwskoZKonkretnymKlimatem, width = 0, kind = TSheetKind.ikDiv10 },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Pipe), desc = CmdCiezkaFajkaOdDziadaMroza, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Tracksuit), desc = CmdKompletUbranFirmySyf, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.SpikedSuit), desc = CmdGarniturZKolcami, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.ManaBottle), desc = CmdMalaButelkeMany, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.LuckyLeaf), desc = CmdDelikatnyChocSzorstkiListek, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.Cassette), desc = CmdKasetaLiroyaLJakLiroy, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.TeleportPill), desc = CmdPigulkaTransportujacaMG, width = 0, kind = TSheetKind.ikPlain },
        new() { item = new IntSlot(() => (short)Bag.Permit, v => Bag.Permit = v), desc = CmdPrzepustkaWlasnoreczniePodpisanaPrzezQuestMastera, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.CompareScroll), desc = CmdScrollZCzaremPorownywanie, width = 0, kind = TSheetKind.ikPlain },
        new() { item = IntSlot.Into(Bag.Items, (int)TBagItem.ReturnScroll), desc = CmdScrollZCzaremPowrot, width = 0, kind = TSheetKind.ikPlain },
    };

    public static void ShowSheetItem(TSheetKind kind, short val, string desc, short width)
    {
        if (val <= -ItemUnit)
            switch (kind)
            {
                case TSheetKind.ikShown:
                    if (width == 0) Out.WriteLine(desc);
                    else Out.WriteLine(desc + val.ToString().PadLeft(width));
                    break;
                case TSheetKind.ikDiv10: Out.WriteLine(desc + (val / 10)); break;
                case TSheetKind.ikPlain: Out.WriteLine(desc + val); break;
            }
    }

    public static void ShowCharacter()
    {
        for (int i = DexterityItemLimits.Length - 1; i >= 1; i--)
            if (Player.Stats[(int)TStat.Dexterity] >= DexterityItemLimits[i].gate)
            {
                Bag.Limit = DexterityItemLimits[i].limit;
                break;
            }
        if (Bag.Items[(int)TBagItem.Backpack] < 0) Bag.Limit = (short)(Bag.Limit + 4);
        Out.WriteLine(TxtShowCharacter_01);
        for (int si = 1; si < SheetItems.Length; si++)
        {
            TSheetItem s = SheetItems[si];
            ShowSheetItem(s.kind, s.item.Get(), s.desc, s.width);
        }
        Out.WriteLine(TxtShowCharacter_02 + Bag.Count + TxtShowCharacter_03 + Bag.Limit + TxtShowCharacter_04);
        if (Gear.Values[(int)TGearItem.WeaponKind] == 0) Out.WriteLine(TxtShowCharacter_05);
        if (Gear.Values[(int)TGearItem.ShieldKind] == 0) Out.WriteLine(TxtShowCharacter_06);
        if (Gear.WornGarment == TxtEmpty) Out.WriteLine(TxtShowCharacter_07);
        if (Gear.WornGarment == TxtWornSyf) Out.WriteLine(TxtWearingLabel + TxtShowCharacter_08);
        if (Gear.WornGarment == TxtWornGarnitur) Out.WriteLine(TxtWearingLabel + TxtShowCharacter_09);
        if ((Gear.Values[(int)TGearItem.ShieldKind] == 1) && (Bag.Items[(int)TBagItem.SmallShield] == -100))
            Out.WriteLine(TxtShowCharacter_10);
        if ((Gear.Values[(int)TGearItem.WeaponKind] == 1) && (Bag.Items[(int)TBagItem.OldSword] == -100))
            Out.WriteLine(TxtShowCharacter_11);
        Out.WriteLine(TxtShowCharacter_12 + Player.Money + TxtShowCharacter_13 + Player.Practice + TxtShared_19);

        short gap = (short)(LevelThreshold(Player.Level) - Player.Experience);
        switch (Player.Level)
        {
            case 1: Out.WriteLine(TxtShowCharacter_18 + TxtLevelFirst + TxtShowCharacter_19 + gap + TxtShared_22); break;
            case 2: Out.WriteLine(TxtShowCharacter_18 + TxtLevelSecond + TxtShowCharacter_19 + gap + TxtShared_22); break;
            case 3: Out.WriteLine(TxtShowCharacter_18 + TxtLevelThird + TxtShowCharacter_19 + gap + TxtShowCharacter_17); break;
            case >= 4 and <= 13: Out.WriteLine(TxtShowCharacter_18 + LevelName(Player.Level) + TxtShowCharacter_19 + gap + TxtShared_22); break;
        }
        if (Player.Level > 13)
            Out.WriteLine(TxtShowCharacter_18 + TxtLevelVeryHigh + TxtShowCharacter_19 + TxtLevelTylko + gap + TxtShared_22);

        Out.WriteLine(TxtShowCharacter_21 + Player.Stats[(int)TStat.Strength] + TxtShowCharacter_03 + Player.Stats[(int)TStat.MaxStrength]
            + TxtShowCharacter_22 + Player.Stats[(int)TStat.Dexterity] + TxtShowCharacter_03 + Player.Stats[(int)TStat.MaxDexterity]
            + TxtShowCharacter_23 + Player.Stats[(int)TStat.Wisdom] + TxtShowCharacter_03 + Player.Stats[(int)TStat.MaxWisdom]);
        Gear.Values[(int)TGearItem.ArmorRating] = (short)(10 * Gear.Values[(int)TGearItem.WornCount] + Gear.Values[(int)TGearItem.Armor]);
        Out.WriteLine(TxtShowCharacter_24 + Gear.Values[(int)TGearItem.ArmorRating]
            + TxtShowCharacter_25 + Gear.Values[(int)TGearItem.LuckRoll]);
        Out.WriteLine(TxtShowCharacter_12 + Player.Mana + TxtShowCharacter_03 + Player.MaxMana + TxtShowCharacter_26);
        Out.WriteLine(TxtShowCharacter_12 + Player.Energy + TxtShowCharacter_03 + Player.MaxEnergy + TxtShared_20);
        if (World.Quest == 1) Out.WriteLine(TxtShowCharacter_27 + World.QuestKills + TxtShowCharacter_28);
        if (World.Quest > 1) Out.WriteLine(TxtShowCharacter_27 + (World.QuestKills - 150) + TxtShowCharacter_28);
        if (World.DuncanQuest != 0) Out.WriteLine(TxtShowCharacter_29);
    }

    public static short FindRace(string name)
    {
        for (int i = 1; i < Races.Length; i++)
            if (name == Races[i].name)
                return (short)i;
        return 0;
    }

    public static void ChooseRace()
    {
        short chosen;
        ShowIntro();
        do
        {
            Out.WriteLine(TxtChooseRace_01);
            Command = Crt.ReadLn();
            chosen = FindRace(Command);
            if (chosen > 0)
            {
                for (int si = 0; si <= (int)TStat.MaxWisdom; si++)
                    Player.Stats[si] = Races[chosen].baseStats[si];
                Player.MaxMana = (short)(Player.MaxMana + Races[chosen].MaxManaBonus);
                Player.Money = Player.Money + Races[chosen].MoneyBonus;
                Player.MaxEnergy = (short)(Player.MaxEnergy + Races[chosen].MaxEnergyBonus);
                Player.Energy = (short)(Player.Energy + Races[chosen].EnergyBonus);
            }
        } while (!(chosen > 0));
        Out.WriteLine(TxtChooseRace_02);
        Command = Crt.ReadLn();
        Player.Energy = 50;
        Player.Practice = 10;
        SpawnAt(CageInits[1..]);
        Player.Level = 1;
        Player.MaxMana = (short)(Player.MaxMana + 100);
        Player.Mana = Player.MaxMana;
        SpawnMonsters();
        WriteLines(TxtChooseRaceLines1[1..]);
    }

    public static void KillStreetFoe(string cmd, ref short target, bool extra, TStreetKind kind)
    {
        if ((Command == cmd) && (target == Session.Room))
        {
            switch (kind)
            {
                case TStreetKind.skDog: FightDog(); break;
                case TStreetKind.skFolk: Fight(TFoe.VeryEasyEnemy); break;
            }
            target = 0;
            if (extra)
                switch (kind)
                {
                    case TStreetKind.skDog: WriteLines(TxtKillDogLines1[1..]); break;
                    case TStreetKind.skFolk: SuitGain(); break;
                }
        }
    }

    public static void KillSweeper()
    {
        if ((Command == CmdZabij + CmdZamiatacz) && (World.Monsters[(int)TMonster.Sweeper] == Session.Room))
        {
            Fight(TFoe.VeryEasyEnemy);
            World.Monsters[(int)TMonster.Sweeper] = 0;
            Combat.Roll = Roll(100);
            if ((Combat.Roll < 35) && (Bag.Items[(int)TBagItem.Tracksuit] == 0))
            {
                Out.WriteLine(TxtKillSweeper_01);
                Bag.Items[(int)TBagItem.Tracksuit] = (short)(Bag.Items[(int)TBagItem.Tracksuit] - ItemUnit);
            }
        }
    }

    public static void KillSickMan()
    {
        if ((Command == CmdZabij + CmdZebrak) && (World.Monsters[(int)TMonster.SickMan] == Session.Room))
        {
            Fight(TFoe.VeryEasyEnemy);
            if (Player.Energy > 0)
            {
                Combat.Roll = Roll(10);
                Out.WriteLine(TxtKillSickMan_01 + Combat.Roll + TxtShared_23);
                Player.Money = Player.Money + Combat.Roll;
                World.Monsters[(int)TMonster.SickMan] = 0;
            }
        }
    }

    public sealed class TStreetKill
    {
        public string cmd = "";
        public TMonster target;
        public bool extra;
        public TStreetKind kind;
    }

    public static readonly TStreetKill[] StreetKills = { null!,
        new() { cmd = CmdZabij + CmdOwczarek, target = TMonster.Shepherd, extra = false, kind = TStreetKind.skDog },
        new() { cmd = CmdZabij + CmdPiesek, target = TMonster.Pooch, extra = false, kind = TStreetKind.skDog },
        new() { cmd = CmdZabij + CmdJamnik, target = TMonster.Dachshund, extra = false, kind = TStreetKind.skDog },
        new() { cmd = CmdZabij + CmdSpaniel, target = TMonster.Spaniel, extra = true, kind = TStreetKind.skDog },
        new() { cmd = CmdZabij + CmdPudel, target = TMonster.Poodle, extra = false, kind = TStreetKind.skDog },
        new() { cmd = CmdZabij + CmdTaksowkarz, target = TMonster.Cabman, extra = true, kind = TStreetKind.skFolk },
        new() { cmd = CmdZabij + CmdSprzedawca, target = TMonster.Vendor, extra = true, kind = TStreetKind.skFolk },
        new() { cmd = CmdZabij + CmdPijak, target = TMonster.Drunkard, extra = false, kind = TStreetKind.skFolk },
    };

    public static void StreetFights()
    {
        for (int ki = 1; ki < StreetKills.Length; ki++)
        {
            TStreetKill k = StreetKills[ki];
            KillStreetFoe(k.cmd, ref World.Monsters[(int)k.target], k.extra, k.kind);
        }
        KillSweeper();
        KillSickMan();
    }

    public sealed class TTrainingLine
    {
        public short wisdom, strength, dexterity;
        public string text1 = "";
    }

    public static readonly TTrainingLine[] TrainingLines = { null!,
        new() { wisdom = 10, strength = 11, dexterity = -1, text1 = TxtTraining_03 },
        new() { wisdom = 10, strength = -1, dexterity = 10, text1 = TxtTraining_04 },
        new() { wisdom = 15, strength = -1, dexterity = 11, text1 = TxtTraining_05 },
        new() { wisdom = 11, strength = -1, dexterity = -1, text1 = TxtTraining_06 },
        new() { wisdom = 18, strength = -1, dexterity = -1, text1 = TxtTraining_07 },
        new() { wisdom = 17, strength = -1, dexterity = -1, text1 = TxtTraining_08 },
        new() { wisdom = -1, strength = -1, dexterity = 20, text1 = TxtTraining_09 },
        new() { wisdom = -1, strength = 20, dexterity = 10, text1 = TxtTraining_10 },
        new() { wisdom = 19, strength = 10, dexterity = 10, text1 = TxtTraining_11 },
        new() { wisdom = 12, strength = 25, dexterity = -1, text1 = TxtTraining_12 },
        new() { wisdom = 14, strength = 29, dexterity = -1, text1 = TxtTraining_13 },
        new() { wisdom = -1, strength = -1, dexterity = 22, text1 = TxtTraining_14 },
        new() { wisdom = -1, strength = -1, dexterity = 28, text1 = TxtTraining_15 },
        new() { wisdom = 22, strength = -1, dexterity = -1, text1 = TxtTraining_16 },
        new() { wisdom = 23, strength = -1, dexterity = -1, text1 = TxtTraining_17 },
        new() { wisdom = 14, strength = 14, dexterity = 24, text1 = TxtTraining_18 },
        new() { wisdom = 24, strength = -1, dexterity = -1, text1 = TxtTraining_19 },
        new() { wisdom = 26, strength = -1, dexterity = -1, text1 = TxtTraining_20 },
        new() { wisdom = 19, strength = 12, dexterity = 12, text1 = TxtTraining_21 },
        new() { wisdom = 16, strength = -1, dexterity = 22, text1 = TxtTraining_22 },
        new() { wisdom = -1, strength = -1, dexterity = 30, text1 = TxtTraining_23 },
        new() { wisdom = 29, strength = -1, dexterity = -1, text1 = TxtTraining_24 },
        new() { wisdom = 19, strength = -1, dexterity = -1, text1 = TxtTraining_25 },
        new() { wisdom = 23, strength = -1, dexterity = 12, text1 = TxtTraining_26 },
    };

    public static bool GatesMet(short reqWisdom, short reqStrength, short reqDexterity) =>
        ((reqWisdom == -1) || (Player.Stats[(int)TStat.Wisdom] > reqWisdom)) &&
        ((reqStrength == -1) || (Player.Stats[(int)TStat.Strength] > reqStrength)) &&
        ((reqDexterity == -1) || (Player.Stats[(int)TStat.Dexterity] > reqDexterity));

    public static void WriteTrainingGain(string skillName, short value)
    {
        Out.WriteLine(TxtTrainingVerb + skillName + TxtTrainingThreshold + value
            + TxtTraining_29 + Player.Practice + TxtShared_19);
    }

    public static void Training()
    {
        if (Command == CmdPatrz + CmdPlakat)
        {
            Out.WriteLine(TxtShared_13);
            Out.WriteLine(TxtTraining_01);
            if (Player.Stats[(int)TStat.Wisdom] < 11) Out.WriteLine(TxtTraining_02);
            for (int i = 1; i < TrainingLines.Length; i++)
                if (GatesMet(TrainingLines[i].wisdom, TrainingLines[i].strength, TrainingLines[i].dexterity))
                    Out.WriteLine(TrainingLines[i].text1);
            Out.WriteLine(TxtTraining_27);
        }

        if (Command == CmdCwicz + CmdKopac)
            if ((Player.Stats[(int)TStat.Wisdom] > 10) && (Player.Stats[(int)TStat.Strength] > 11) && (Player.Practice > 0))
            {
                Skills.Values[(int)TSkill.Kick] = (short)(Skills.Values[(int)TSkill.Kick] + Player.Stats[(int)TStat.Wisdom]);
                Player.Practice = (short)(Player.Practice - 1);
                WriteTrainingGain(TxtTrainingDigging, Skills.Values[(int)TSkill.Kick]);
            }

        if (Command == CmdCwicz + CmdUciekac)
        {
            if (Player.Stats[(int)TStat.Wisdom] > 10)
                if (Player.Stats[(int)TStat.Dexterity] > 10)
                    if ((Player.Practice > 0) && (Skills.Values[(int)TSkill.Fleeing] < 85))
                    {
                        Skills.Values[(int)TSkill.Fleeing] = (short)(Skills.Values[(int)TSkill.Fleeing] + Player.Stats[(int)TStat.Wisdom] + Player.Stats[(int)TStat.Dexterity] - 5);
                        Player.Practice = (short)(Player.Practice - 1);
                        WriteTrainingGain(TxtTrainingFleeing, Skills.Values[(int)TSkill.Fleeing]);
                    }
                    else
                        Out.WriteLine(TxtTraining_31);
        }

        if (Command == CmdCwicz + CmdPowrot)
            if ((Player.Stats[(int)TStat.Wisdom] > 17) && (Skills.Values[(int)TSkill.Returning] < 90) && (Player.Practice > 0))
            {
                Skills.Values[(int)TSkill.Returning] = (short)(Skills.Values[(int)TSkill.Returning] + 2 * Player.Stats[(int)TStat.Wisdom] - 3);
                Player.Practice = (short)(Player.Practice - 1);
                WriteTrainingGain(TxtTrainingReturning, Skills.Values[(int)TSkill.Returning]);
            }

        if (Command == CmdCwicz + TxtTrainingParrying)
            if ((Player.Stats[(int)TStat.Wisdom] > 15) && (Player.Stats[(int)TStat.Dexterity] > 11) && (Skills.Values[(int)TSkill.Parry] < 90) && (Player.Practice > 0))
            {
                Skills.Values[(int)TSkill.Parry] = (short)(Skills.Values[(int)TSkill.Parry] + Player.Stats[(int)TStat.Wisdom] + Player.Stats[(int)TStat.Dexterity] - 14);
                Player.Practice = (short)(Player.Practice - 1);
                WriteTrainingGain(TxtTrainingParrying, Skills.Values[(int)TSkill.Parry]);
            }

        if (Command == CmdCwicz + CmdPorownanie)
            if ((Player.Stats[(int)TStat.Wisdom] > 11) && (Skills.Values[(int)TSkill.Comparing] < 90) && (Player.Practice > 0))
            {
                Skills.Values[(int)TSkill.Comparing] = (short)(Skills.Values[(int)TSkill.Comparing] + 3 * Player.Stats[(int)TStat.Wisdom] - 9);
                Player.Practice = (short)(Player.Practice - 1);
                WriteTrainingGain(TxtTrainingComparing, Skills.Values[(int)TSkill.Comparing]);
            }

        if (Command == CmdCwicz + TxtTrainingCooking)
            if ((Player.Stats[(int)TStat.Wisdom] > 18) && (Skills.Values[(int)TSkill.Cooking] < 90) && (Player.Practice > 0))
            {
                Skills.Values[(int)TSkill.Cooking] = (short)(Skills.Values[(int)TSkill.Cooking] + Player.Stats[(int)TStat.Wisdom] + 1);
                Player.Practice = (short)(Player.Practice - 1);
                WriteTrainingGain(TxtTrainingCooking, Skills.Values[(int)TSkill.Cooking]);
            }
    }

    // Pascal `Text` file: raw bytes, decimal tokens separated by
    // whitespace, `\n` line ends. Read(integer) stops before the
    // terminator; Read(string) runs to end-of-line; bare ReadLn
    // consumes through the next line end.
    public sealed class SaveFile
    {
        private string _name = "";
        private FileStream? _stream;
        private readonly List<byte> _pushback = new();

        public void Assign(string name) => _name = name;
        public void Rewrite() => _stream = File.Create(_name);
        public void Reset() => _stream = File.OpenRead(_name);

        public void Close()
        {
            _stream?.Close();
            _stream = null;
            _pushback.Clear();
        }

        private int GetByte()
        {
            if (_pushback.Count > 0)
            {
                int last = _pushback[^1];
                _pushback.RemoveAt(_pushback.Count - 1);
                return last;
            }
            return _stream!.ReadByte();
        }

        public void WriteLine(long value) => WriteLine(value.ToString());

        public void WriteLine(string value)
        {
            foreach (char c in value)
                _stream!.WriteByte((byte)c);
            _stream!.WriteByte((byte)'\n');
            _stream.Flush();
        }

        private static bool IsSpace(int b) => b == ' ' || b == '\t' || b == '\n' || b == '\r';

        public long ReadNumber()
        {
            int b;
            do { b = GetByte(); } while (b >= 0 && IsSpace(b));
            bool neg = false;
            if (b == '+' || b == '-')
            {
                neg = b == '-';
                b = GetByte();
            }
            long value = 0;
            while (b >= '0' && b <= '9')
            {
                value = value * 10 + (b - '0');
                b = GetByte();
            }
            _pushback.Add((byte)b);
            return neg ? -value : value;
        }

        public string ReadRemainder()
        {
            StringBuilder sb = new();
            int b;
            while ((b = GetByte()) >= 0 && b != '\n')
                sb.Append((char)b);
            if (b == '\n')
                _pushback.Add((byte)b);
            return sb.ToString();
        }

        public void SkipLine()
        {
            int b;
            do { b = GetByte(); } while (b >= 0 && b != '\n');
        }
    }

    public static void Assign(SaveFile f, string name) => f.Assign(name);
    public static void Rewrite(SaveFile f) => f.Rewrite();
    public static void Reset(SaveFile f) => f.Reset();
    public static void Close(SaveFile f) => f.Close();

    public static void SaveSlot(int value) => Session.SaveFile.WriteLine((long)value);
    public static void SaveSlotText(string value) => Session.SaveFile.WriteLine(value);
    public static void LoadSlot(ref short value) => value = unchecked((short)Session.SaveFile.ReadNumber());
    public static void LoadSlotShort(ref sbyte value) => value = unchecked((sbyte)Session.SaveFile.ReadNumber());
    public static void LoadSlotLong(ref int value) => value = unchecked((int)Session.SaveFile.ReadNumber());
    public static void LoadSlotText(ref string value) => value = Session.SaveFile.ReadRemainder();

    public static void SaveCharacter()
    {
        Command = CmdJa;
        Session.SavedFlag = 1;
        Out.WriteLine(TxtSaveCharacter_01);
        Crt.ClrScr();
        Assign(Session.SaveFile, TxtSaveFileName);
        Rewrite(Session.SaveFile);
        SaveSlot(Player.Mana * SaveStatScale);
        SaveSlot(Player.Level + SaveLevelBias);
        SaveSlot(Bag.Items[(int)TBagItem.OldSword]);
        SaveSlot(Bag.Items[(int)TBagItem.Pipe]);
        SaveSlot(Bag.Items[(int)TBagItem.Diploma]);
        SaveSlot(Bag.Items[(int)TBagItem.SmallShield]);
        SaveSlot(Bag.Items[(int)TBagItem.SpikedSuit]);
        SaveSlot(Player.Experience);
        SaveSlot((Player.Energy + SaveEnergyBias) * SaveStatScale);
        SaveSlot(Session.OuterRoom);
        SaveSlot(Player.Stats[(int)TStat.Dexterity] + SaveDexterityBias);
        SaveSlot(Player.Stats[(int)TStat.Strength] - SaveStrengthBias);
        SaveSlot(Player.Stats[(int)TStat.Wisdom]);
        SaveSlot(Player.Practice);
        SaveSlot(Session.CheckpointStage);
        SaveSlot(0);
        SaveSlot(0);
        SaveSlot(Player.Money * Player.Stats[(int)TStat.Wisdom]);
        SaveSlot(Combat.Loot);
        SaveSlot(Player.MaxEnergy);
        SaveSlot(Bag.Limit);
        SaveSlot(Skills.Values[(int)TSkill.Comparing]);
        SaveSlot(Player.MaxMana);
        SaveSlot(Player.Stats[(int)TStat.MaxStrength]);
        SaveSlot(Player.Stats[(int)TStat.MaxDexterity]);
        SaveSlot(Player.Stats[(int)TStat.MaxWisdom]);
        SaveSlot(Bag.Count);
        SaveSlot(Gear.Values[(int)TGearItem.Armor]);
        SaveSlot(Gear.Values[(int)TGearItem.LuckRoll]);
        SaveSlot(Gear.Values[(int)TGearItem.WeaponKind]);
        SaveSlot(Gear.Values[(int)TGearItem.Wearing]);
        SaveSlot(Session.Overall);
        SaveSlot(Session.SavedFlag);
        SaveSlot(World.Monsters[(int)TMonster.CageWeakling]);
        SaveSlot(World.Monsters[(int)TMonster.CageStrong]);
        SaveSlot(World.Monsters[(int)TMonster.CageAgile]);
        SaveSlot(World.Monsters[(int)TMonster.CageTough]);
        SaveSlot(World.Monsters[(int)TMonster.CageHuge]);
        SaveSlot(Gear.Values[(int)TGearItem.ShieldKind]);
        SaveSlot(Gear.Values[(int)TGearItem.ArmorRating]);
        SaveSlot(Skills.Values[(int)TSkill.Cooking]);
        SaveSlot(Skills.Values[(int)TSkill.CookingDisplayBug]);
        SaveSlot(0);
        SaveSlot(Bag.Items[(int)TBagItem.Heart]);
        SaveSlot(Bag.Items[(int)TBagItem.ManaBottle]);
        SaveSlot(Bag.Items[(int)TBagItem.Cookie]);
        SaveSlot(Bag.Items[(int)TBagItem.DryRation]);
        SaveSlot(Bag.Items[(int)TBagItem.Bread]);
        SaveSlot(Bag.Items[(int)TBagItem.BreadRoll]);
        SaveSlot(Bag.Items[(int)TBagItem.Sausage]);
        SaveSlot(Bag.Items[(int)TBagItem.Bread]);
        SaveSlot(Bag.Items[(int)TBagItem.BreadRoll]);
        SaveSlot(Gear.Values[(int)TGearItem.WornCount]);
        SaveSlot(Skills.Values[(int)TSkill.Parry]);
        SaveSlot(Skills.Values[(int)TSkill.Kick]);
        SaveSlot(Skills.Values[(int)TSkill.KickMinMana]);
        SaveSlot(Skills.Values[(int)TSkill.KickMaxEnergy]);
        SaveSlot(Skills.Values[(int)TSkill.Fleeing]);
        SaveSlot(Skills.Values[(int)TSkill.FleeEnergy]);
        SaveSlot(Combat.Fled);
        SaveSlot(Session.InArena);
        SaveSlot(Bag.Items[(int)TBagItem.Tracksuit]);
        SaveSlot(Bag.Items[(int)TBagItem.Cassette]);
        SaveSlot(Bag.Items[(int)TBagItem.TeleportPill]);
        SaveSlot(World.Monsters[(int)TMonster.Woodworm]);
        SaveSlot(World.Monsters[(int)TMonster.Fly]);
        SaveSlot(World.Monsters[(int)TMonster.Bacterium]);
        SaveSlot(World.Quest);
        SaveSlot(World.QuestKills);
        SaveSlot(World.Monsters[(int)TMonster.Snail]);
        SaveSlot(Bag.Items[(int)TBagItem.LuckyLeaf]);
        SaveSlot(Skills.Values[(int)TSkill.Returning]);
        SaveSlot(Bag.Items[(int)TBagItem.Bigos]);
        SaveSlot(Bag.Items[(int)TBagItem.Beer]);
        SaveSlot(Bag.Items[(int)TBagItem.CompareScroll]);
        SaveSlot(Bag.Items[(int)TBagItem.ReturnScroll]);
        SaveSlot(World.OldElf);
        SaveSlot(World.DuncanQuest);
        SaveSlot(Bag.Items[(int)TBagItem.Backpack]);
        SaveSlot(World.Monsters[(int)TMonster.Duncan]);
        SaveSlotText(Gear.WornGarment);
        Close(Session.SaveFile);
    }

    public static void Death()
    {
        WriteLines(TxtDeathLines1[1..]);
        if (World.Quest == 1) World.QuestKills = 50;
        if (World.Quest > 1) World.QuestKills = 200;
        Player.Energy = Player.MaxEnergy;
        Player.Experience = (short)(Player.Experience - 250 + Roll(50) - (5 * Player.Level));
        Session.Room = RoomCityCentre;
        SpawnMonsters();
        Command = CmdPamietaj;
        Crt.ReadLn();
        SaveCharacter();
    }

    public static void BuyItem(string cmd, ref short Item, short price, string itemName, bool available)
    {
        if ((Command == cmd) && available && (Player.Money >= price))
        {
            Bag.Count = (short)(Bag.Count + 1);
            Item = (short)(Item - ItemUnit);
            Player.Money = Player.Money - price;
            Out.WriteLine(TxtShopVerb + itemName + TxtSpaceZa + price + TxtSpaceMonet);
        }
    }

    public sealed class TBakeryItem
    {
        public string cmd = "";
        public TBagItem item;
        public short price;
        public string desc = "";
    }

    public static readonly TBakeryItem[] BakeryItems = { null!,
        new() { cmd = CmdKup + CmdPaczek, item = TBagItem.Doughnut, price = 8, desc = TxtItemPaczek },
        new() { cmd = CmdKup + CmdCiastko, item = TBagItem.Cookie, price = 12, desc = TxtItemCiastko },
        new() { cmd = CmdKup + CmdSuchaRacja, item = TBagItem.DryRation, price = 15, desc = TxtItemSuchaRacje },
        new() { cmd = CmdKup + CmdBulka, item = TBagItem.BreadRoll, price = 19, desc = TxtItemBulke },
        new() { cmd = CmdKup + CmdChleb, item = TBagItem.Bread, price = 24, desc = TxtItemChleb },
        new() { cmd = CmdKup + CmdWeka, item = TBagItem.Sausage, price = 29, desc = TxtItemWeke },
    };

    public static void BakeryShop()
    {
        if (Command == CmdLista)
            WriteLines(TxtBakeryShopLines1[1..]);
        StreetFights();
        for (int bi2 = 1; bi2 < BakeryItems.Length; bi2++)
        {
            TBakeryItem b = BakeryItems[bi2];
            BuyItem(b.cmd, ref Bag.Items[(int)b.item], b.price, b.desc, true);
        }
    }

    public sealed class TShopRule
    {
        public string cmd = "";
        public Func<bool> allowed = () => false;
        public Action act = () => { };
    }

    public static void RunShopRules(TShopRule[] rules)
    {
        for (int i = 0; i < rules.Length; i++)
            if (Command == rules[i].cmd)
            {
                if (rules[i].allowed())
                    rules[i].act();
                break;
            }
    }

    public static bool Always() => true;

    public static bool CanBuyOldSword() =>
        (Bag.Items[(int)TBagItem.OldSword] == 0) && (Player.Money >= 70);

    public static void SellItem(ref short Item, short price, string itemName)
    {
        Out.WriteLine(TxtSellVerb + itemName);
        Player.Money = Player.Money + price;
        Item = (short)(Item + ItemUnit);
        Bag.Count = (short)(Bag.Count - 1);
    }

    public static void BuyOldSword() =>
        BuyItem(CmdKup + CmdStary, ref Bag.Items[(int)TBagItem.OldSword], 70, TxtBuyOldSword_01, true);

    public static bool CanSellOldSword() => Bag.Items[(int)TBagItem.OldSword] == -ItemUnit;

    public static void SellOldSword()
    {
        Out.WriteLine(TxtSellOldSword_01);
        Out.WriteLine(TxtSellOldSword_02);
        Player.Money = Player.Money + 30;
        Bag.Items[(int)TBagItem.OldSword] = 0;
        Bag.Count = (short)(Bag.Count - 1);
    }

    public static bool CanBuySmallShield() =>
        (Bag.Items[(int)TBagItem.SmallShield] == 0) && (Player.Money >= 15);

    public static void BuySmallShield() =>
        BuyItem(CmdKup + CmdMala, ref Bag.Items[(int)TBagItem.SmallShield], 15, TxtBuySmallShield_01, true);

    public static void SellSmallShield()
    {
        Out.WriteLine(TxtSellSmallShield_01);
        Player.Money = Player.Money + 8;
        Bag.Items[(int)TBagItem.SmallShield] = 0;
        Bag.Count = (short)(Bag.Count - 1);
    }

    public static readonly TShopRule[] ArmouryRules = {
        new() { cmd = CmdKup + CmdStary, allowed = CanBuyOldSword, act = BuyOldSword },
        new() { cmd = CmdSprzedaj + CmdStary, allowed = CanSellOldSword, act = SellOldSword },
        new() { cmd = CmdKup + CmdMala, allowed = CanBuySmallShield, act = BuySmallShield },
        new() { cmd = CmdSprzedaj + CmdMala, allowed = Always, act = SellSmallShield },
    };

    public static bool CanBuyPipe() =>
        (Bag.Items[(int)TBagItem.Pipe] == 0) && (Player.Money >= 480);

    public static void BuyPipe()
    {
        Out.WriteLine(TxtShopVerb + TxtBuyPipe_01 + TxtSpaceZa + 480 + TxtSpaceMonet);
        Player.Money = Player.Money - 480;
        Bag.Items[(int)TBagItem.Pipe] = (short)(Bag.Items[(int)TBagItem.Pipe] - ItemUnit);
        Bag.Count = (short)(Bag.Count + 1);
        Player.Stats[(int)TStat.Wisdom]++;
    }

    public static bool CanBuyTracksuit() =>
        (Bag.Items[(int)TBagItem.Tracksuit] == 0) && (Player.Money >= 35);

    public static void BuyTracksuit() =>
        BuyItem(CmdKup + CmdKomplet, ref Bag.Items[(int)TBagItem.Tracksuit], 35, TxtBuyTracksuit_01, true);

    public static bool CanSellTracksuit() =>
        (Bag.Items[(int)TBagItem.Tracksuit] <= -ItemUnit) && (Gear.WornGarment != TxtWornSyf);

    public static void SellTracksuit() =>
        SellItem(ref Bag.Items[(int)TBagItem.Tracksuit], 15, TxtSellTracksuit_01);

    public static bool CanSellPipe() => Bag.Items[(int)TBagItem.Pipe] == -ItemUnit;

    public static void SellPipe()
    {
        Out.WriteLine(TxtSellVerb + TxtSellPipe_01);
        Player.Money = Player.Money + 120;
        Bag.Items[(int)TBagItem.Pipe] = (short)(Bag.Items[(int)TBagItem.Pipe] + ItemUnit);
        Bag.Count = (short)(Bag.Count - 1);
        Player.Stats[(int)TStat.Wisdom]--;
    }

    public static bool CanBuyCassette() => Player.Money >= 1999;

    public static void BuyCassette()
    {
        Out.WriteLine(TxtShopVerb + TxtBuyCassette_01 + TxtSpaceZa + 1999 + TxtSpaceMonet);
        Player.Money = Player.Money - 1999;
        Bag.Items[(int)TBagItem.Cassette] = (short)(Bag.Items[(int)TBagItem.Cassette] - ItemUnit);
        Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] - 8);
        Player.MaxEnergy = (short)(Player.MaxEnergy + 5);
        Player.Stats[(int)TStat.Dexterity]++;
        Bag.Count = (short)(Bag.Count + 1);
    }

    public static bool CanBuySpikedSuit() => Player.Money >= 1000;

    public static void BuySpikedSuit()
    {
        Out.WriteLine(TxtBuySpikedSuit_01);
        Player.Money = Player.Money - 1000;
        Bag.Items[(int)TBagItem.SpikedSuit] = (short)(Bag.Items[(int)TBagItem.SpikedSuit] - ItemUnit);
        Bag.Count = (short)(Bag.Count + 1);
    }

    public static bool CanSellSpikedSuit() => Gear.WornGarment != TxtWornGarnitur;

    public static void SellSpikedSuit() =>
        SellItem(ref Bag.Items[(int)TBagItem.SpikedSuit], 500, TxtSellSpikedSuit_01);

    public static bool CanSellCassette() => Bag.Items[(int)TBagItem.Cassette] < 0;

    public static void SellCassette()
    {
        Out.WriteLine(TxtSellVerb + TxtSellCassette_01);
        Player.Money = Player.Money + 1000;
        Bag.Items[(int)TBagItem.Cassette] = (short)(Bag.Items[(int)TBagItem.Cassette] + ItemUnit);
        Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] + 8);
        Player.MaxEnergy = (short)(Player.MaxEnergy - 5);
        Player.Stats[(int)TStat.Dexterity]--;
        Bag.Count = (short)(Bag.Count - 1);
    }

    public static readonly TShopRule[] GeneralStoreRules = {
        new() { cmd = CmdKup + CmdFajka, allowed = CanBuyPipe, act = BuyPipe },
        new() { cmd = CmdKup + CmdKomplet, allowed = CanBuyTracksuit, act = BuyTracksuit },
        new() { cmd = CmdSprzedaj + CmdKomplet, allowed = CanSellTracksuit, act = SellTracksuit },
        new() { cmd = CmdSprzedaj + CmdFajka, allowed = CanSellPipe, act = SellPipe },
        new() { cmd = CmdKup + CmdKaseta, allowed = CanBuyCassette, act = BuyCassette },
        new() { cmd = CmdKup + TxtWornGarnitur, allowed = CanBuySpikedSuit, act = BuySpikedSuit },
        new() { cmd = CmdSprzedaj + TxtWornGarnitur, allowed = CanSellSpikedSuit, act = SellSpikedSuit },
        new() { cmd = CmdSprzedaj + CmdKaseta, allowed = CanSellCassette, act = SellCassette },
    };

    public static bool CanBuyManaBottle() => Player.Money >= 15;

    public static void BuyManaBottle()
    {
        Out.WriteLine(TxtBuyManaBottle_01);
        Player.Money = Player.Money - 20;
        Bag.Items[(int)TBagItem.ManaBottle] = (short)(Bag.Items[(int)TBagItem.ManaBottle] - ItemUnit);
        Bag.Count = (short)(Bag.Count + 1);
    }

    public static bool CanBuyLuckyLeaf() => Player.Money >= 820;

    public static void BuyLuckyLeaf()
    {
        Out.WriteLine(TxtBuyLuckyLeaf_01);
        Player.Money = Player.Money - 830;
        Bag.Items[(int)TBagItem.LuckyLeaf] = (short)(Bag.Items[(int)TBagItem.LuckyLeaf] - ItemUnit);
        Bag.Count = (short)(Bag.Count + 1);
        Player.MaxMana = (short)(Player.MaxMana + 40);
    }

    public static bool CanSellLuckyLeaf() => Player.Money >= 820;

    public static void SellLuckyLeaf()
    {
        Out.WriteLine(TxtSellVerb + TxtSellLuckyLeaf_01);
        Player.Money = Player.Money + 400;
        Bag.Items[(int)TBagItem.LuckyLeaf] = (short)(Bag.Items[(int)TBagItem.LuckyLeaf] + ItemUnit);
        Bag.Count = (short)(Bag.Count - 1);
        Player.MaxMana = (short)(Player.MaxMana - 40);
    }

    public static bool CanBuyTeleportPill() => Player.Money >= 235;

    public static void BuyTeleportPill() =>
        BuyItem(CmdKup + CmdPigulkaTransportujaca, ref Bag.Items[(int)TBagItem.TeleportPill], 235, TxtBuyTeleportPill_01, true);

    public static bool CanSellTeleportPill() => Bag.Items[(int)TBagItem.TeleportPill] <= 10;

    public static void SellTeleportPill()
    {
        Out.WriteLine(TxtSellTeleportPill_01);
        Player.Money = Player.Money + 170;
        Bag.Items[(int)TBagItem.TeleportPill] = (short)(Bag.Items[(int)TBagItem.TeleportPill] + ItemUnit);
        Bag.Count = (short)(Bag.Count - 1);
    }

    public static bool CanBuyCompareScroll() => Player.Money > 39;

    public static void BuyCompareScroll()
    {
        Out.WriteLine(TxtBuyCompareScroll_01);
        Player.Money = Player.Money - 40;
        Bag.Items[(int)TBagItem.CompareScroll] = (short)(Bag.Items[(int)TBagItem.CompareScroll] - ItemUnit);
        Bag.Count = (short)(Bag.Count + 1);
    }

    public static bool CanSellCompareScroll() => Bag.Items[(int)TBagItem.CompareScroll] < 0;

    public static void SellCompareScroll()
    {
        Out.WriteLine(TxtSellCompareScroll_01);
        Player.Money = Player.Money + 18;
        Bag.Items[(int)TBagItem.CompareScroll] = (short)(Bag.Items[(int)TBagItem.CompareScroll] + ItemUnit);
        Bag.Count = (short)(Bag.Count - 1);
    }

    public static readonly TShopRule[] MagicShopRules = {
        new() { cmd = CmdKup + CmdMalaButelkaMany, allowed = CanBuyManaBottle, act = BuyManaBottle },
        new() { cmd = CmdKup + CmdListek, allowed = CanBuyLuckyLeaf, act = BuyLuckyLeaf },
        new() { cmd = CmdSprzedaj + CmdListek, allowed = CanSellLuckyLeaf, act = SellLuckyLeaf },
        new() { cmd = CmdKup + CmdPigulkaTransportujaca, allowed = CanBuyTeleportPill, act = BuyTeleportPill },
        new() { cmd = CmdSprzedaj + CmdPigulkaTransportujaca, allowed = CanSellTeleportPill, act = SellTeleportPill },
        new() { cmd = CmdKup + CmdScrollPorownywanie, allowed = CanBuyCompareScroll, act = BuyCompareScroll },
        new() { cmd = CmdSprzedaj + CmdScrollPorownywanie, allowed = CanSellCompareScroll, act = SellCompareScroll },
    };

    public sealed class TShopStock
    {
        public TBagItem item;
        public string[] lines = { "", "", "", "" };
    }

    public static readonly TShopStock[] ArmouryStock = { null!,
        new() { item = TBagItem.OldSword, lines = new string[] { "", TxtArmouryShop_01, "", "" } },
        new() { item = TBagItem.SmallShield, lines = new string[] { "", TxtArmouryShop_02, "", "" } },
    };

    public static readonly TShopStock[] GeneralStoreStock = { null!,
        new() { item = TBagItem.Pipe, lines = new string[] { "", TxtGeneralStoreShop_02, "", "" } },
        new() { item = TBagItem.Tracksuit, lines = new string[] { "", TxtGeneralStoreShop_03, TxtGeneralStoreShop_04, TxtGeneralStoreShop_05 } },
    };

    public static void ShowStock(TBagItem item, string[] lines)
    {
        if (Bag.Items[(int)item] == 0)
            for (int si = 0; si < lines.Length; si++)
                if (lines[si] != "") Out.WriteLine(lines[si]);
    }

    public static void ArmouryShop()
    {
        if (Command == CmdLista)
        {
            Out.WriteLine(TxtShared_24);
            for (int si2 = 1; si2 < ArmouryStock.Length; si2++)
                ShowStock(ArmouryStock[si2].item, ArmouryStock[si2].lines[1..]);
            Out.WriteLine(TxtArmouryShop_03);
        }
        StreetFights();
        RunShopRules(ArmouryRules);
    }

    public static void GeneralStoreShop()
    {
        if (Command == CmdLista)
        {
            Out.WriteLine(TxtGeneralStoreShop_01);
            for (int si2 = 1; si2 < GeneralStoreStock.Length; si2++)
                ShowStock(GeneralStoreStock[si2].item, GeneralStoreStock[si2].lines[1..]);
        }
        if (Command == CmdPatrz + CmdPoster)
            Out.WriteLine(TxtGeneralStoreShop_06);
        RunShopRules(GeneralStoreRules);
    }

    public static void MagicShop()
    {
        if (Command == CmdLista)
            WriteLines(TxtMagicShopLines1[1..]);
        RunShopRules(MagicShopRules);
    }

    public enum TConcertBonus { cbNone, cbPipe, cbSuit }

    public sealed class TConcertKill
    {
        public string cmd = "";
        public TMonster monster;
        public TFoe foe;
        public TConcertBonus bonus;
    }

    public static readonly TConcertKill[] ConcertKills = { null!,
        new() { cmd = CmdZabij + CmdDziecko, monster = TMonster.Kid, foe = TFoe.WeakishEnemy, bonus = TConcertBonus.cbNone },
        new() { cmd = CmdZabij + CmdDziadek, monster = TMonster.OldMan, foe = TFoe.WeakishEnemy, bonus = TConcertBonus.cbPipe },
        new() { cmd = CmdZabij + CmdWariat, monster = TMonster.Lunatic, foe = TFoe.MediumEnemy, bonus = TConcertBonus.cbNone },
        new() { cmd = CmdZabij + CmdSluchacz, monster = TMonster.Listener, foe = TFoe.MediumEnemy, bonus = TConcertBonus.cbNone },
        new() { cmd = CmdZabij + CmdCzlowiek, monster = TMonster.Man, foe = TFoe.MediumEnemy, bonus = TConcertBonus.cbNone },
        new() { cmd = CmdZabij + CmdFan, monster = TMonster.Fan, foe = TFoe.MediumEnemy, bonus = TConcertBonus.cbNone },
        new() { cmd = CmdZabij + CmdReporter, monster = TMonster.Reporter, foe = TFoe.MediumEnemy, bonus = TConcertBonus.cbNone },
        new() { cmd = CmdZabij + CmdPolicjant, monster = TMonster.Policeman, foe = TFoe.HardEnemy, bonus = TConcertBonus.cbNone },
        new() { cmd = CmdZabij + CmdGoryl, monster = TMonster.Gorilla, foe = TFoe.HardEnemy, bonus = TConcertBonus.cbSuit },
        new() { cmd = CmdZabij + CmdOchroniarz, monster = TMonster.Bouncer, foe = TFoe.HardEnemy, bonus = TConcertBonus.cbSuit },
    };

    public static void ConcertKill(string cmd, TMonster monster, TFoe foe, TConcertBonus bonus)
    {
        if ((Command == cmd) && (Session.Room == World.Monsters[(int)monster]))
        {
            Fight(foe);
            if (Combat.EnemyEnergy < 1)
            {
                World.Monsters[(int)monster] = 0;
                if (bonus == TConcertBonus.cbPipe)
                {
                    Combat.Roll = Roll(100);
                    if ((Combat.Roll < 5) && (Bag.Items[(int)TBagItem.Pipe] == 0))
                    {
                        Out.WriteLine(TxtShared_08 + TxtConcertFights_01);
                        Out.WriteLine(TxtConcertFights_02);
                        Bag.Items[(int)TBagItem.Pipe] = (short)(Bag.Items[(int)TBagItem.Pipe] - ItemUnit);
                        Player.Stats[(int)TStat.Wisdom]++;
                    }
                }
            }
            if (bonus == TConcertBonus.cbSuit) SuitGain();
        }
    }

    public static void ConcertFights()
    {
        for (int ci = 1; ci < ConcertKills.Length; ci++)
        {
            TConcertKill c = ConcertKills[ci];
            ConcertKill(c.cmd, c.monster, c.foe, c.bonus);
        }
    }

    public static readonly TCheckpoint[] CheckpointRules = { null!,
        new(400, 0, 1, (short)(-MaxInt - 1), 19),
        new(180, 0, 1, (short)(-MaxInt - 1), 7),
        new(360, 1, 2, (short)(-MaxInt - 1), 7),
        new(540, 2, 3, (short)(-MaxInt - 1), 7),
        new(300, 0, 1, 8, 12),
        new(500, 1, 2, 8, 12),
    };

    public static void ReachCheckpoint(short exp, short stage, short next, short loStr, short hiStr)
    {
        if ((Player.Experience >= exp) && (Session.CheckpointStage == stage)
            && (Player.Stats[(int)TStat.Strength] >= loStr) && (Player.Stats[(int)TStat.Strength] <= hiStr))
        {
            Out.WriteLine(TxtCheckpoints_01);
            Session.CheckpointStage = next;
            SpawnMonsters();
        }
    }

    public static void Checkpoints()
    {
        for (int ci = 1; ci < CheckpointRules.Length; ci++)
        {
            TCheckpoint c = CheckpointRules[ci];
            ReachCheckpoint(c.exp, c.stage, c.next, c.loStr, c.hiStr);
        }
    }

    public sealed class TEnergyBonus
    {
        public short dexLo, strLo, bonus;
    }

    public static readonly TBand[] LevelCosts = { null!,
        new(1, 1, 725, 0),
        new(2, 2, 730, 0),
        new(3, 3, 735, 0),
        new(4, 8, Lvl4_Base, 1),
        new(9, MaxInt, Lvl9_Base, 2),
    };

    public static readonly TBand[] PracticeGains = { null!,
        new(-128, 10, 3, 0),
        new(11, 15, 4, 0),
        new(16, 22, 5, 0),
        new((short)(-MaxInt - 1), MaxInt, 6, 0),
    };

    public static readonly TEnergyBonus[] MaxEnergyGains = { null!,
        new() { dexLo = 12, strLo = (short)(-MaxInt - 1), bonus = 1 },
        new() { dexLo = 16, strLo = (short)(-MaxInt - 1), bonus = 1 },
        new() { dexLo = 21, strLo = (short)(-MaxInt - 1), bonus = 1 },
        new() { dexLo = 26, strLo = 20, bonus = 1 },
    };

    public static void CheckLevelUp()
    {
        short threshold;
        if (Player.Level < 1) return;
        threshold = LevelThreshold(Player.Level);
        if (Player.Experience < threshold) return;

        WriteLines(TxtCheckLevelUpLines1[1..]);

        Player.Experience = (short)(Player.Experience - BandAmount(LevelCosts[1..], Player.Level));
        Player.Practice = (short)(Player.Practice + BandAmount(PracticeGains[1..], Player.Stats[(int)TStat.Wisdom]));
        Out.WriteLine(TxtCheckLevelUp_02 + Player.Practice + TxtCheckLevelUp_03);
        Player.MaxEnergy = (short)(Player.MaxEnergy + 10);
        for (int ei = 1; ei < MaxEnergyGains.Length; ei++)
        {
            TEnergyBonus e = MaxEnergyGains[ei];
            if ((Player.Stats[(int)TStat.Dexterity] > e.dexLo) && (Player.Stats[(int)TStat.Strength] > e.strLo))
                Player.MaxEnergy = (short)(Player.MaxEnergy + e.bonus);
        }
        Out.WriteLine(TxtCheckLevelUp_02 + Player.MaxEnergy + TxtCheckLevelUp_04);
        Player.MaxMana = (short)(Player.MaxMana + Player.Stats[(int)TStat.Wisdom] + 2);
        Out.WriteLine(TxtCheckLevelUp_05 + (Player.Stats[(int)TStat.Wisdom] + 2) + TxtCheckLevelUp_06);
        Player.Level = (sbyte)(Player.Level + 1);
        Session.CheckpointStage = 0;
        if (Player.Level == 12)
        {
            Out.WriteLine(TxtCheckLevelUp_07);
            if (Player.Stats[(int)TStat.Strength] >= Player.Stats[(int)TStat.MaxStrength]) Player.Stats[(int)TStat.MaxStrength] = (short)(Player.Stats[(int)TStat.MaxStrength] + 3);
            if (Player.Stats[(int)TStat.Dexterity] >= Player.Stats[(int)TStat.MaxDexterity]) Player.Stats[(int)TStat.MaxDexterity] = (short)(Player.Stats[(int)TStat.MaxDexterity] + 3);
            if (Player.Stats[(int)TStat.Wisdom] >= Player.Stats[(int)TStat.MaxWisdom]) Player.Stats[(int)TStat.MaxWisdom] = (short)(Player.Stats[(int)TStat.MaxWisdom] + 3);
            if (Player.Stats[(int)TStat.Strength] < Player.Stats[(int)TStat.MaxStrength]) Player.Stats[(int)TStat.MaxStrength] = (short)(Player.Stats[(int)TStat.MaxStrength] + 2);
            if (Player.Stats[(int)TStat.Dexterity] < Player.Stats[(int)TStat.MaxDexterity]) Player.Stats[(int)TStat.MaxDexterity] = (short)(Player.Stats[(int)TStat.MaxDexterity] + 2);
            if (Player.Stats[(int)TStat.Wisdom] < Player.Stats[(int)TStat.MaxWisdom]) Player.Stats[(int)TStat.MaxWisdom] = (short)(Player.Stats[(int)TStat.MaxWisdom] + 2);
            if ((Player.Stats[(int)TStat.MaxWisdom] > Player.Stats[(int)TStat.MaxStrength]) && (Player.Stats[(int)TStat.MaxWisdom] > Player.Stats[(int)TStat.MaxDexterity]))
                Player.Stats[(int)TStat.MaxWisdom] = (short)(Player.Stats[(int)TStat.MaxWisdom] + 3);
            if ((Player.Stats[(int)TStat.MaxStrength] > Player.Stats[(int)TStat.MaxWisdom]) && (Player.Stats[(int)TStat.MaxStrength] > Player.Stats[(int)TStat.MaxDexterity]))
                Player.Stats[(int)TStat.MaxStrength] = (short)(Player.Stats[(int)TStat.MaxStrength] + 3);
            if ((Player.Stats[(int)TStat.MaxDexterity] > Player.Stats[(int)TStat.MaxStrength]) && (Player.Stats[(int)TStat.MaxDexterity] > Player.Stats[(int)TStat.MaxWisdom]))
                Player.Stats[(int)TStat.MaxDexterity] = (short)(Player.Stats[(int)TStat.MaxDexterity] + 3);
            if ((Player.Stats[(int)TStat.MaxDexterity] == Player.Stats[(int)TStat.MaxStrength]) && (Player.Stats[(int)TStat.MaxStrength] == Player.Stats[(int)TStat.MaxWisdom]))
            {
                Player.Stats[(int)TStat.MaxDexterity] = (short)(Player.Stats[(int)TStat.MaxDexterity] + 1);
                Player.Stats[(int)TStat.MaxWisdom] = (short)(Player.Stats[(int)TStat.MaxWisdom] + 1);
                Player.Stats[(int)TStat.MaxStrength] = (short)(Player.Stats[(int)TStat.MaxStrength] + 1);
            }
            if ((Player.Stats[(int)TStat.MaxDexterity] > Player.Stats[(int)TStat.MaxWisdom]) && (Player.Stats[(int)TStat.MaxDexterity] == Player.Stats[(int)TStat.MaxStrength]))
            {
                Player.Stats[(int)TStat.MaxStrength] = (short)(Player.Stats[(int)TStat.MaxStrength] + 2);
                Player.Stats[(int)TStat.MaxDexterity] = (short)(Player.Stats[(int)TStat.MaxDexterity] + 1);
            }
            Out.WriteLine(TxtCheckLevelUp_08 + Player.Stats[(int)TStat.MaxStrength]
                + TxtMaxymalnie + TxtCheckLevelUp_09);
            Out.WriteLine(TxtCheckLevelUp_08 + Player.Stats[(int)TStat.MaxDexterity]
                + TxtMaxymalnie + TxtCheckLevelUp_10);
            Out.WriteLine(TxtCheckLevelUp_08 + Player.Stats[(int)TStat.MaxWisdom]
                + TxtMaxymalnie + TxtCheckLevelUp_11);
        }
    }

    public static void CmdScan() => Out.WriteLine(TxtCmdScan_01);
    public static void CmdClearScreen() => Crt.ClrScr();

    public static void CmdGiveEnergy() => Player.Energy = (short)(Player.Energy + 200);
    public static void CmdTakeMoney() => Player.Money = Player.Money - 100;

    public static void CmdResetArmor()
    {
        Gear.Values[(int)TGearItem.WornCount] = 0;
        Gear.Values[(int)TGearItem.Armor] = 0;
        Gear.Values[(int)TGearItem.LuckRoll] = 0;
    }

    public static void CmdStrip()
    {
        Gear.Values[(int)TGearItem.Wearing] = 0;
        Gear.Values[(int)TGearItem.WeaponKind] = 0;
        Gear.Values[(int)TGearItem.ShieldKind] = 0;
        Gear.WornGarment = TxtEmpty;
        Bag.Items[(int)TBagItem.OldSword] = -ItemUnit;
        Bag.Items[(int)TBagItem.SmallShield] = -ItemUnit;
        Player.Stats[(int)TStat.Strength]--;
    }

    public static void CmdNoExit() => Out.WriteLine(TxtCmdNoExit_01);
    public static void CmdQuitGame() => Session.ExitRequested = true;

    public static void CmdJustSaved()
    {
        if (Session.SavedFlag == 1)
        {
            Command = CmdZdolnosci;
            Session.SavedFlag = 0;
            Out.WriteLine(Session.OuterRoom + TxtCmdJustSaved_01);
        }
    }

    public static void CmdDrinkBeer()
    {
        if (Bag.Items[(int)TBagItem.Beer] <= -ItemUnit)
        {
            Bag.Items[(int)TBagItem.Beer] = (short)(Bag.Items[(int)TBagItem.Beer] + ItemUnit);
            Out.WriteLine(TxtCmdDrinkBeer_01);
            Bag.Count = (short)(Bag.Count - 1);
            Player.Energy = (short)(Player.Energy + 10);
            Player.Mana = (short)(Player.Mana + 10);
        }
    }

    public static void CmdWieldOldSword()
    {
        if ((Bag.Items[(int)TBagItem.OldSword] == -ItemUnit) && (Gear.Values[(int)TGearItem.WeaponKind] == 0))
        {
            Bag.Items[(int)TBagItem.OldSword] = -100;
            Gear.Values[(int)TGearItem.WeaponKind] = 1;
            Out.WriteLine(TxtCmdWieldOldSword_01);
            Player.Stats[(int)TStat.Strength]++;
            Gear.Values[(int)TGearItem.LuckRoll] = (short)(Gear.Values[(int)TGearItem.LuckRoll] + 3);
        }
    }

    public static void CmdUnwieldOldSword()
    {
        if ((Bag.Items[(int)TBagItem.OldSword] == -100) && (Gear.Values[(int)TGearItem.WeaponKind] == 1))
        {
            Bag.Items[(int)TBagItem.OldSword] = -ItemUnit;
            Gear.Values[(int)TGearItem.WeaponKind] = 0;
            Out.WriteLine(TxtCmdUnwieldOldSword_01);
            Player.Stats[(int)TStat.Strength]--;
            Gear.Values[(int)TGearItem.LuckRoll] = (short)(Gear.Values[(int)TGearItem.LuckRoll] - 3);
        }
    }

    public static void CmdWieldSmallShield()
    {
        if ((Gear.Values[(int)TGearItem.ShieldKind] == 0) && (Bag.Items[(int)TBagItem.SmallShield] == -ItemUnit))
        {
            Gear.Values[(int)TGearItem.ShieldKind] = 1;
            Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] + 15);
            Gear.Values[(int)TGearItem.WornCount] = (short)(Gear.Values[(int)TGearItem.WornCount] + 1);
            Bag.Items[(int)TBagItem.SmallShield] = -100;
            Out.WriteLine(TxtCmdWieldSmallShield_01);
        }
    }

    public static void CmdUnwieldSmallShield()
    {
        if ((Gear.Values[(int)TGearItem.ShieldKind] == 1) && (Bag.Items[(int)TBagItem.SmallShield] == -100))
        {
            Gear.Values[(int)TGearItem.ShieldKind] = 0;
            Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] - 15);
            Gear.Values[(int)TGearItem.WornCount] = (short)(Gear.Values[(int)TGearItem.WornCount] - 1);
            Bag.Items[(int)TBagItem.SmallShield] = -ItemUnit;
            Out.WriteLine(TxtCmdUnwieldSmallShield_01);
        }
    }

    public static void CmdSetFleeEnergy()
    {
        if (Skills.Values[(int)TSkill.Fleeing] > 0)
        {
            Out.WriteLine(TxtCmdSetFleeEnergy_01);
            Skills.Values[(int)TSkill.FleeEnergy] = Crt.ReadShort();
        }
    }

    public static void CmdSleep()
    {
        Session.SleepHours = 0;
        do
        {
            Session.SleepHours = (short)(Session.SleepHours + 1);
            Crt.Delay(5000);
            Out.WriteLine(TxtCmdSleep_01 + Session.SleepHours + TxtCmdSleep_02);
            Player.Experience = (short)(Player.Experience - 20);
            Player.Energy = (short)(Player.Energy + 10);
        } while (!Crt.KeyPressed());
        Out.WriteLine(TxtCmdSleep_03 + (Session.SleepHours * 10)
            + TxtCmdSleep_04 + ((20 * Session.SleepHours) - Roll(2 * Session.SleepHours)) + TxtShared_22);
        Player.Experience = (short)(Player.Experience + Roll(2 * Session.SleepHours));
        if (Session.SleepHours > 4)
        {
            Out.WriteLine(TxtCmdSleep_05 + Player.Stats[(int)TStat.Wisdom] + TxtCmdSleep_06);
            Player.Energy = (short)(Player.Energy + Player.Stats[(int)TStat.Wisdom]);
        }
        if (Session.SleepHours > 8)
        {
            Out.WriteLine(TxtCmdSleep_07 + (Player.Stats[(int)TStat.Wisdom] * 2) + TxtShared_19);
            Player.Energy = (short)(Player.Energy + (Player.Stats[(int)TStat.Wisdom] * 2));
        }
        short tmpEnergy = Player.Energy;
        ClampStat(ref tmpEnergy, Player.MaxEnergy);
        Player.Energy = tmpEnergy;
    }

    public static void CmdCompare()
    {
        if (Skills.Values[(int)TSkill.Comparing] > 0)
        {
            Combat.Roll = Roll(100);
            if (Skills.Values[(int)TSkill.Comparing] > Combat.Roll)
            {
                CompareFoe();
                Player.Mana = (short)(Player.Mana - 10);
            }
            else
            {
                Out.WriteLine(TxtCmdCompare_01);
                Player.Mana = (short)(Player.Mana - 5);
            }
        }
    }

    public static void CmdGiveMoney() => Player.Money = Player.Money + 5000;

    public static readonly string[] RefusedFood = { "",
        CmdOdrzuc + CmdPaczek, CmdOdrzuc + CmdCiastko,
        CmdOdrzuc + CmdSuchaRacja, CmdOdrzuc + CmdBulka,
        CmdOdrzuc + CmdChleb, CmdOdrzuc + CmdWeka,
        CmdOdrzuc + CmdSerce, CmdOdrzuc + CmdBigos };

    public static void CmdRefuseFoodDrop()
    {
        for (int i = 1; i < RefusedFood.Length; i++)
            if (Command == RefusedFood[i])
            {
                Out.WriteLine(TxtCmdRefuseFoodDrop_01);
                break;
            }
    }

    public static void CmdOpenPortal()
    {
        if (((Command == CmdPowrot) && (Skills.Values[(int)TSkill.Returning] > 0)) ||
            ((Command == CmdUzyj + CmdScrollPowrot) && (Bag.Items[(int)TBagItem.ReturnScroll] < 0)))
        {
            Combat.Roll = Roll(100);
            if (Command == CmdUzyj + CmdScrollPowrot)
            {
                Combat.Roll = Roll(1);
                Bag.Items[(int)TBagItem.ReturnScroll] = (short)(Bag.Items[(int)TBagItem.ReturnScroll] + ItemUnit);
                Bag.Count = (short)(Bag.Count - 1);
            }
            if (Combat.Roll <= Skills.Values[(int)TSkill.Returning])
            {
                Out.WriteLine(TxtCmdOpenPortal_01);
                Player.Mana = (short)(Player.Mana - 15);
                Session.OuterRoom = RoomCityCentre;
            }
            if ((Combat.Roll > Skills.Values[(int)TSkill.Returning]) && (Skills.Values[(int)TSkill.Returning] + 30 > Combat.Roll))
            {
                Out.WriteLine(TxtCmdOpenPortal_02);
                Player.Mana = (short)(Player.Mana - 5);
            }
            if (Skills.Values[(int)TSkill.Returning] + 29 < Combat.Roll)
            {
                Out.WriteLine(TxtCmdOpenPortal_03);
                Player.Mana = (short)(Player.Mana - 10);
                Session.OuterRoom = (short)(Roll(100) + 1);
            }
        }
    }

    public static void CmdUseCompareScroll()
    {
        if (Bag.Items[(int)TBagItem.CompareScroll] < 0)
        {
            CompareFoe();
            Player.Mana = (short)(Player.Mana - 10);
            Bag.Items[(int)TBagItem.CompareScroll] = (short)(Bag.Items[(int)TBagItem.CompareScroll] + ItemUnit);
            Bag.Count = (short)(Bag.Count - 1);
        }
    }

    public static void CmdSetKickThresholds()
    {
        if (Skills.Values[(int)TSkill.Kick] > 0)
        {
            Out.WriteLine(TxtCmdSetKickThresholds_01);
            Out.WriteLine(TxtCmdSetKickThresholds_02);
            Skills.Values[(int)TSkill.KickMaxEnergy] = Crt.ReadShort();
            Out.WriteLine(TxtCmdSetKickThresholds_03);
            Skills.Values[(int)TSkill.KickMinMana] = Crt.ReadShort();
        }
    }

    public static void CmdSpeede() => Bag.Permit = Bag.Permit - ItemUnit;

    public sealed class TSkillLine
    {
        public short wisdom, strength, dexterity;
        public string text1 = "";
        public TSkill skill;
        public string text2 = "";
    }

    public static readonly TSkillLine[] SkillLines = { null!,
        new() { wisdom = 10, strength = 11, dexterity = -1, text1 = TxtCmdShowSkills_01, skill = TSkill.Kick, text2 = TxtShared_26 },
        new() { wisdom = 10, strength = -1, dexterity = 10, text1 = TxtCmdShowSkills_02, skill = TSkill.Fleeing, text2 = TxtShared_26 },
        new() { wisdom = 15, strength = -1, dexterity = 11, text1 = TxtCmdShowSkills_03, skill = TSkill.Parry, text2 = TxtShared_26 },
        new() { wisdom = 11, strength = -1, dexterity = -1, text1 = TxtCmdShowSkills_04, skill = TSkill.Comparing, text2 = TxtShared_26 },
        new() { wisdom = 18, strength = -1, dexterity = -1, text1 = TxtCmdShowSkills_05, skill = TSkill.CookingDisplayBug, text2 = TxtShared_26 },
        new() { wisdom = 19, strength = -1, dexterity = -1, text1 = TxtCmdShowSkills_06, skill = TSkill.Returning, text2 = TxtShared_26 },
    };

    public static void CmdShowSkills()
    {
        for (int i = 1; i < SkillLines.Length; i++)
            if (GatesMet(SkillLines[i].wisdom, SkillLines[i].strength, SkillLines[i].dexterity))
            {
                TSkillLine s = SkillLines[i];
                Out.WriteLine(s.text1 + Skills.Values[(int)s.skill] + s.text2);
            }
    }

    public static void CmdLeaveTrance() => Session.Room = Session.OuterRoom;

    public static readonly TCommandEntry[] GroundCommands = {
        new(CmdPamietaj, SaveCharacter),
        new(CmdScan2, CmdScan),
        new(CmdCzysc + CmdEkran, CmdClearScreen),
        new(CmdDawaj + CmdEn, CmdGiveEnergy),
        new(CmdZabieraj + CmdKase, CmdTakeMoney),
        new(CmdUstaw + CmdSz, CmdResetArmor),
        new(CmdZdejmuj, CmdStrip),
        new(CmdExit, CmdNoExit),
        new(CmdWyjscie, CmdQuitGame),
        new(CmdJa, ShowCharacter),
        new(TxtEmpty, TakeDrop),
        new(TxtEmpty, CmdJustSaved),
        new(CmdUzyj + CmdPiwo, CmdDrinkBeer),
        new(CmdUzyj + CmdStary, CmdWieldOldSword),
        new(CmdOdloz + CmdStary, CmdUnwieldOldSword),
        new(CmdUzyj + CmdMala, CmdWieldSmallShield),
        new(CmdOdloz + CmdMala, CmdUnwieldSmallShield),
        new(TxtEmpty, UseItems),
        new(CmdZwiej, CmdSetFleeEnergy),
        new(CmdSpij, CmdSleep),
        new(CmdPorownaj, CmdCompare),
        new(CmdDawaj + CmdKase, CmdGiveMoney),
        new(TxtEmpty, CmdRefuseFoodDrop),
        new(TxtEmpty, CmdOpenPortal),
        new(CmdUzyj + CmdScrollPorownywanie, CmdUseCompareScroll),
        new(CmdKop, CmdSetKickThresholds),
        new(CmdJestem + CmdSpeede2, CmdSpeede),
        new(CmdZdolnosci, CmdShowSkills),
        new(TxtEmpty, DisplayPrefs),
        new(CmdWlacz + CmdPostac, LoadCharacter),
        new(CmdUnmode, CmdLeaveTrance),
        new(CmdUm, CmdLeaveTrance),
    };

    public static void Start()
    {
        Randomize();
        ChooseRace();
        Crt.ReadKey();
        do
        {
            Checkpoints();
            CheckLevelUp();

            if (Bag.Count > Bag.Limit)
            {
                Out.WriteLine(TxtStart_01 + (Bag.Count - Bag.Limit) + TxtStart_02);
                Out.WriteLine(TxtStart_03 + ((Bag.Count - Bag.Limit) * 10) + TxtStart_04);
                Player.Energy = (short)(Player.Energy - ((Bag.Count - Bag.Limit) * 10));
            }

            if (Combat.PoisonRounds > 0)
            {
                Combat.Roll = Roll(5);
                Out.WriteLine(TxtStart_05 + Combat.Roll + TxtStart_06);
                Combat.PoisonRounds = (sbyte)(Combat.PoisonRounds - 1);
            }

            Session.ExitRequested = false;
            DispatchRooms();
            if (Session.ExitRequested) return;

            ArenaMaze();
            if (Session.Room == DeadRoom) Death();
            short tmpEnergy = Player.Energy;
            ClampStat(ref tmpEnergy, Player.MaxEnergy);
            Player.Energy = tmpEnergy;
            if (Session.Room == TranceRoom)
            {
                for (int ti = 1; ti < TranceItems.Length; ti++)
                {
                    TTranceItem t = TranceItems[ti];
                    TranceItemLine(t.item.Get(), t.line);
                }
                do
                {
                    ReadCommand();

                    Session.ExitRequested = false;
                    for (int gi = 0; gi < GroundCommands.Length; gi++)
                        if ((GroundCommands[gi].cmd == TxtEmpty) || (Command == GroundCommands[gi].cmd))
                        {
                            GroundCommands[gi].run();
                            if (Session.ExitRequested) return;
                        }
                } while (!(Session.Room != TranceRoom));
            }

            Combat.Fled = 0;

        } while (!(Session.Room == RoomQuit));
    }

    public static void LoadCharacter()
    {
        short dummy = 0;
        Command = Crt.ReadLn();
        Assign(Session.SaveFile, TxtSaveFileName);
        Reset(Session.SaveFile);
        LoadSlot(ref Player.Mana);
        Player.Mana = (short)(Player.Mana / SaveStatScale);
        LoadSlotShort(ref Player.Level);
        Player.Level = (sbyte)(Player.Level - SaveLevelBias);
        LoadSlot(ref Bag.Items[(int)TBagItem.OldSword]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Pipe]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Diploma]);
        LoadSlot(ref Bag.Items[(int)TBagItem.SmallShield]);
        LoadSlot(ref Bag.Items[(int)TBagItem.SpikedSuit]);
        LoadSlot(ref Player.Experience);
        LoadSlot(ref Player.Energy);
        Player.Energy = (short)(Player.Energy / SaveStatScale);
        Player.Energy = (short)(Player.Energy - SaveEnergyBias);
        LoadSlot(ref Session.OuterRoom);
        LoadSlot(ref Player.Stats[(int)TStat.Dexterity]);
        Player.Stats[(int)TStat.Dexterity] = (short)(Player.Stats[(int)TStat.Dexterity] - SaveDexterityBias);
        LoadSlot(ref Player.Stats[(int)TStat.Strength]);
        Player.Stats[(int)TStat.Strength] = (short)(Player.Stats[(int)TStat.Strength] + SaveStrengthBias);
        LoadSlot(ref Player.Stats[(int)TStat.Wisdom]);
        LoadSlot(ref Player.Practice);
        LoadSlot(ref Session.CheckpointStage);
        LoadSlot(ref dummy);
        LoadSlot(ref dummy);
        LoadSlotLong(ref Player.Money);
        Player.Money = Player.Money / Player.Stats[(int)TStat.Wisdom];
        LoadSlot(ref Combat.Loot);
        LoadSlot(ref Player.MaxEnergy);
        LoadSlot(ref Bag.Limit);
        LoadSlot(ref Skills.Values[(int)TSkill.Comparing]);
        LoadSlot(ref Player.MaxMana);
        LoadSlot(ref Player.Stats[(int)TStat.MaxStrength]);
        LoadSlot(ref Player.Stats[(int)TStat.MaxDexterity]);
        LoadSlot(ref Player.Stats[(int)TStat.MaxWisdom]);
        LoadSlot(ref Bag.Count);
        LoadSlot(ref Gear.Values[(int)TGearItem.Armor]);
        LoadSlot(ref Gear.Values[(int)TGearItem.LuckRoll]);
        LoadSlot(ref Gear.Values[(int)TGearItem.WeaponKind]);
        LoadSlot(ref Gear.Values[(int)TGearItem.Wearing]);
        LoadSlot(ref Session.Overall);
        LoadSlot(ref Session.SavedFlag);
        LoadSlot(ref World.Monsters[(int)TMonster.CageWeakling]);
        LoadSlot(ref World.Monsters[(int)TMonster.CageStrong]);
        LoadSlot(ref World.Monsters[(int)TMonster.CageAgile]);
        LoadSlot(ref World.Monsters[(int)TMonster.CageTough]);
        LoadSlot(ref World.Monsters[(int)TMonster.CageHuge]);
        LoadSlot(ref Gear.Values[(int)TGearItem.ShieldKind]);
        LoadSlot(ref Gear.Values[(int)TGearItem.ArmorRating]);
        LoadSlot(ref Skills.Values[(int)TSkill.Cooking]);
        LoadSlot(ref Skills.Values[(int)TSkill.CookingDisplayBug]);
        LoadSlot(ref dummy);
        LoadSlot(ref Bag.Items[(int)TBagItem.Heart]);
        LoadSlot(ref Bag.Items[(int)TBagItem.ManaBottle]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Cookie]);
        LoadSlot(ref Bag.Items[(int)TBagItem.DryRation]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Bread]);
        LoadSlot(ref Bag.Items[(int)TBagItem.BreadRoll]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Sausage]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Bread]);
        LoadSlot(ref Bag.Items[(int)TBagItem.BreadRoll]);
        LoadSlot(ref Gear.Values[(int)TGearItem.WornCount]);
        LoadSlot(ref Skills.Values[(int)TSkill.Parry]);
        LoadSlot(ref Skills.Values[(int)TSkill.Kick]);
        LoadSlot(ref Skills.Values[(int)TSkill.KickMinMana]);
        LoadSlot(ref Skills.Values[(int)TSkill.KickMaxEnergy]);
        LoadSlot(ref Skills.Values[(int)TSkill.Fleeing]);
        LoadSlot(ref Skills.Values[(int)TSkill.FleeEnergy]);
        LoadSlot(ref Combat.Fled);
        LoadSlot(ref Session.InArena);
        LoadSlot(ref Bag.Items[(int)TBagItem.Tracksuit]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Cassette]);
        LoadSlot(ref Bag.Items[(int)TBagItem.TeleportPill]);
        LoadSlot(ref World.Monsters[(int)TMonster.Woodworm]);
        LoadSlot(ref World.Monsters[(int)TMonster.Fly]);
        LoadSlot(ref World.Monsters[(int)TMonster.Bacterium]);
        LoadSlot(ref World.Quest);
        LoadSlot(ref World.QuestKills);
        LoadSlot(ref World.Monsters[(int)TMonster.Snail]);
        LoadSlot(ref Bag.Items[(int)TBagItem.LuckyLeaf]);
        LoadSlot(ref Skills.Values[(int)TSkill.Returning]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Bigos]);
        LoadSlot(ref Bag.Items[(int)TBagItem.Beer]);
        LoadSlot(ref Bag.Items[(int)TBagItem.CompareScroll]);
        LoadSlot(ref Bag.Items[(int)TBagItem.ReturnScroll]);
        LoadSlot(ref World.OldElf);
        LoadSlotShort(ref World.DuncanQuest);
        LoadSlot(ref Bag.Items[(int)TBagItem.Backpack]);
        LoadSlot(ref World.Monsters[(int)TMonster.Duncan]);
        Session.SaveFile.SkipLine();
        LoadSlotText(ref Gear.WornGarment);
        Close(Session.SaveFile);
        Out.WriteLine(TxtLoadCharacter_01 + Gear.WornGarment);
    }

    // __MORE__
}
