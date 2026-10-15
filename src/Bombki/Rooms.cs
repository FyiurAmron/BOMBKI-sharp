// Literal port of src-pas/Rooms.pas. `using static Game` keeps the
// room bodies line-close to the Pascal originals.

using static Game;
using static Game.TMonster;
using static Game.TFoe;
using static Game.TStreetKind;

static class Rooms
{
    public static void Room0()
    {
        Out.WriteLine(TxtRoom0_01);
        do
        {
            ReadCommand();
            if (Command == CmdExit) Out.WriteLine(TxtRoom0_02);
            if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomSalon) })) return;
        } while (!(Session.Room != RoomStart));
    }

    public static void Room1()
    {
        BasicRoomPoster(RoomSalon,
                  new[] { TxtYouAreIn + TxtRoom1_01, TxtRoom1Lines1[1], TxtRoom1Lines1[2],
                   TxtRoom1Lines1[3] },
                  new[] { TxtShared_03,
                   TxtExitPolodnie, TxtPlaceTamGdzieZaczynaszGre,
                   TxtExitPolnoc, TxtPlaceMiasto,
                   TxtExitWschod, TxtPlacePokojTreningowy1,
                   TxtExitZachod, TxtPlaceMudSzkola,
                   TxtExitDol, TxtPlacePoddziemnyPokojTreningowy2 },
                  new[] { Go(CmdPolodnie, RoomStart),
                   Go(CmdWschod, RoomTraining),
                   Go(CmdDol, RoomMeditation),
                   Go(CmdZachod, RoomSchoolHall),
                   Go(CmdPolnoc, RoomCityCentre) },
                  TxtRoom1Lines2[1..]);
    }

    public static void Room2()
    {
        WriteLines(new[] { TxtYouAreIn + TxtRoom2_01, TxtRoom2_02, TxtShared_04 });
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitZachod, TxtPlacePok224JCentralny });
            if (MoveWhenOrExit(new[] { Go(CmdZachod, RoomSalon) })) return;
            if (Command == CmdPatrz + CmdPlakat)
                WriteLines(TxtRoom2Lines1[1..]);
            HandleTrain();
        } while (!(Session.Room != RoomTraining));
    }

    public static void Room3()
    {
        WriteLines(TxtRoom3Lines1[1..]);
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                Out.WriteLine(TxtRoom3_04);
            if (MoveWhenOrExit(new[] { Go(CmdGora, RoomSalon) })) return;
            Training();
        } while (!(Session.Room != RoomMeditation));
    }

    public static void Room4()
    {
        BasicRoomGeneric(RoomSchoolHall,
                  new[] { TxtYouAreIn + TxtRoom4_01, TxtRoom4_02 },
                  new[] { TxtShared_03,
                   TxtExitWschod, TxtPlacePokojCentralny,
                   TxtExitZachod, TxtPlaceMudSzkola2 },
                  new[] { Go(CmdWschod, RoomSalon), Go(CmdZachod, RoomLost) },
                  CmdPatrz + CmdAfisz,
                  TxtRoom4Lines1[1..]);
    }

    public static void Room5()
    {
        BasicRoomPoster(RoomLost,
                  TxtRoom5Lines1[1..],
                  new[] { TxtShared_03,
                   TxtExitWschod, TxtPlaceMudSzkola,
                   TxtExitZachod, TxtPlacePokoj,
                   TxtExitPolnoc, TxtPlacePokoj,
                   TxtExitPolodnie, TxtPlacePokoj,
                   TxtExitGora, TxtPlacePokoj,
                   TxtExitDol, TxtPlacePokoj },
                  new[] { Go(CmdWschod, RoomSchoolHall),
                   Go(CmdZachod, RoomSchoolSide1),
                   Go(CmdPolnoc, RoomSchoolSide2),
                   Go(CmdPolodnie, RoomSchoolSide3),
                   Go(CmdGora, RoomSchoolLanding),
                   Go(CmdDol, RoomSchoolSide4) },
                  new[] { TxtRoom5_10, TxtRoom5_11 });
    }

    public static void Room9()
    {
        BasicRoomSimple(RoomSchoolLanding,
                  new[] { TxtYouAreIn + TxtRoom9_01, TxtRoom9_02 },
                  new[] { TxtShared_03,
                   TxtExitDol, TxtPlaceMudSzkola2,
                   TxtExitGora, TxtPlaceKlatkiPelneGajdy },
                  new[] { Go(CmdDol, RoomLost), Go(CmdGora, RoomCageCorridor) });
    }

    public static void Room11()
    {
        Out.WriteLine(TxtYouAreIn + TxtRoom11_01);
        WriteLines(TxtRoom11Lines1[1..]);
        do
        {
            ReadCommand();
            ExpandAbbrev();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceKlatkaCieniasa,
                            TxtExitZachod, TxtPlaceKlatkaSilnego,
                            TxtExitPolnoc, TxtPlaceKlatkaZrecznego,
                            TxtExitPolodnie, TxtPlaceKlatkaOdpornego,
                            TxtExitGora, TxtPlaceKlatkaSilnegoOdpornegoIZrecznego,
                            TxtExitDol, TxtPlaceJedynaDrogaNieProwadzacaDoKlatek });
            if (MoveWhenOrExit(new[] { Go(CmdWschod, RoomCageWeakling),
                      Go(CmdZachod, RoomCageStrong),
                      Go(CmdPolnoc, RoomCageAgile),
                      Go(CmdPolodnie, RoomCageTough),
                      Go(CmdGora, RoomCageHuge),
                      Go(CmdDol, RoomCageReward) })) return;
            if (Command == CmdPatrz + CmdPlakat)
                WriteLines(TxtRoom11Lines2[1..]);
        } while (!(Session.Room != RoomCageCorridor));
    }

    public static void CageMonsterFight(ref short monster, short room, short energy, short dexterity, short strength, short lootRand, short lootThreshold)
    {
        if ((Command == CmdZabij + CmdPotwor) && (monster == room))
        {
            Combat.EnemyEnergy = energy; Combat.EnemyDexterity = dexterity; Combat.EnemyStrength = strength;
            DoFight();
            if (Combat.Fled == 0)
            {
                monster = 0;
                Combat.Loot = Roll(lootRand);
                Out.WriteLine(TxtShared_08 + Combat.Loot + TxtShared_09);
                Player.Money = Player.Money + Combat.Loot;
                TryCageLoot(lootThreshold);
            }
            else
                Combat.Fled = 0;
        }
    }

    public static void DescribeCage(TMonster monster, short room, string aliveLine, string deadLine)
    {
        Out.WriteLine(TxtShared_06);
        if (World.Monsters[(int)monster] == room) Out.WriteLine(aliveLine);
        else if (World.Monsters[(int)monster] == 0) Out.WriteLine(deadLine);
    }

    public static void Room12()
    {
        DescribeCage(CageWeakling, RoomCageWeakling, TxtRoom12_01, TxtShared_07);
        do
        {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitZachod, TxtPlaceKlatkiPelneGajdy });
            CheckMode();
            if (MoveWhenOrExit(new[] { Go(CmdZachod, RoomCageCorridor) })) return;
            CageMonsterFight(ref World.Monsters[(int)CageWeakling], RoomCageWeakling, 20, 3, 3, 10, 4);
        } while (!(Session.Room != RoomCageWeakling));
    }

    public static void Room13()
    {
        DescribeCage(CageStrong, RoomCageStrong, TxtRoom13_01, TxtShared_07);
        do
        {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceKlatkiPelneGajdy });
            CheckMode();
            if (MoveWhenOrExit(new[] { Go(CmdWschod, RoomCageCorridor) })) return;
            CageMonsterFight(ref World.Monsters[(int)CageStrong], RoomCageStrong, 20, 3, 10, 15, 7);
        } while (!(Session.Room != RoomCageStrong));
    }

    public static void Room14()
    {
        DescribeCage(CageAgile, RoomCageAgile, TxtRoom14_01, TxtRoom14_02);
        do
        {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlaceKlatkiPelneGajdy });
            CheckMode();
            if (MoveWhenOrQuit(new[] { Go(CmdPolodnie, RoomCageCorridor) })) return;
            CageMonsterFight(ref World.Monsters[(int)CageAgile], RoomCageAgile, 20, 30, 3, 15, 7);
        } while (!(Session.Room != RoomCageAgile));
    }

    public static void Room15()
    {
        DescribeCage(CageTough, RoomCageTough, TxtRoom15_01, TxtShared_07);
        do
        {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolnoc, TxtPlaceKlatkiPelneGajdy });
            CheckMode();
            if (MoveWhenOrQuit(new[] { Go(CmdPolnoc, RoomCageCorridor) })) return;
            CageMonsterFight(ref World.Monsters[(int)CageTough], RoomCageTough, 40, 3, 3, 15, 7);
        } while (!(Session.Room != RoomCageTough));
    }

    public static void Room16()
    {
        DescribeCage(CageHuge, RoomCageHuge, TxtRoom16_01, TxtShared_07);
        do
        {
            if (World.Monsters[(int)CageHuge] == RoomCageHuge)
            {
                Combat.EnemyEnergy = 40; Combat.EnemyDexterity = 11; Combat.EnemyStrength = 10;
                DoFight();
                if (Combat.Fled != 0)
                {
                    Combat.Fled = 0;
                    Session.Room = RoomCageCorridor;
                }
                else
                {
                    World.Monsters[(int)CageHuge] = 0;
                    Combat.Loot = Roll(30);
                    Out.WriteLine(TxtShared_08 + Combat.Loot + TxtShared_09);
                    Player.Money = Player.Money + Combat.Loot;
                    TryCageLoot(10);
                }
            }
            ReadCommand();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitDol, TxtPlaceKlatkiPelneGajdy });
            CheckMode();
            if (MoveWhenOrQuit(new[] { Go(CmdDol, RoomCageCorridor) })) return;
        } while (!(Session.Room != RoomCageHuge));
    }

    public static void Room17()
    {
        Out.WriteLine(TxtYouAreIn + TxtRoom17_01);
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitGora, TxtPlaceKlatkiPelneGajdy,
                            TxtExitDol, TxtPlaceTeleport });
            if (Command == CmdPatrz + CmdPlakat)
            {
                WriteLines(TxtRoom17Lines1[1..]);
                if ((World.Monsters[(int)CageWeakling] == 0) && (World.Monsters[(int)CageAgile] == 0) &&
                (World.Monsters[(int)CageStrong] == 0) && (World.Monsters[(int)CageTough] == 0) &&
                (World.Monsters[(int)CageHuge] == 0) && (Bag.Items[(int)TBagItem.Diploma] == 0))
                    Bag.Items[(int)TBagItem.Diploma] = RoomCageReward;
            }
            if (MoveWhenOrQuit(new[] { Go(CmdGora, RoomCageCorridor), Go(CmdDol, RoomTeleport) })) return;
        } while (!(Session.Room != RoomCageReward));
    }

    public static void Room18()
    {
        WriteLines(TxtRoom18Lines1[1..]);
        Session.Room = RoomSalon;
    }

    public static void Room20()
    {
        Out.WriteLine(TxtYouAreIn + TxtRoom20_01);
        DescribeStreet();
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdPatrz + CmdPlakat)
                Out.WriteLine(TxtRoom20_02);
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlacePokojCentralny,
                            TxtExitPolnoc, TxtPlaceUlSklepikowa,
                            TxtExitWschod, TxtPlaceUlCiemna,
                            TxtExitZachod, TxtPlaceUlDluga });
            if (Command == CmdWyjsc0ie) { Session.ExitRequested = true; return; }
            MoveWhen(new[] { Go(CmdPolodnie, RoomSalon),
                      Go(CmdPolnoc, RoomShopStreet),
                      Go(CmdZachod, RoomLongStreet),
                      Go(CmdWschod, RoomDarkStreet) });
            StreetFights();
        } while (!(Session.Room != RoomCityCentre));
    }

    public static void Room21()
    {
        Out.WriteLine(TxtRoom21_01);
        Out.WriteLine(TxtRoom21_02);
        DescribeStreet();
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlaceCentrumMiasta,
                            TxtExitPolnoc, TxtPlaceUlSklepikowa,
                            TxtExitWschod, TxtPlacePiekarnia,
                            TxtExitZachod, TxtPlaceZbrojownia });
            if (MoveWhenOrQuit(new[] { Go(CmdPolodnie, RoomCityCentre),
                      Go(CmdPolnoc, RoomShopStreet2),
                      Go(CmdWschod, RoomBakery),
                      Go(CmdZachod, RoomArmoury) })) return;
            StreetFights();
        } while (!(Session.Room != RoomShopStreet));
    }

    public static void Room22()
    {
        Out.WriteLine(TxtRoom22_01);
        Out.WriteLine(TxtRoom22_02);
        DescribeStreet();
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlaceUlSklepikowa,
                            TxtExitPolnoc, TxtPlaceRozjazd,
                            TxtExitWschod, TxtPlaceSklepWielobranzowy,
                            TxtExitZachod, TxtPlaceSklepMagiczny });
            if (MoveWhenOrQuit(new[] { Go(CmdPolodnie, RoomShopStreet),
                      Go(CmdWschod, RoomGeneralStore),
                      Go(CmdZachod, RoomMagicShop),
                      Go(CmdPolnoc, RoomQuestMaster) })) return;
            StreetFights();
        } while (!(Session.Room != RoomShopStreet2));
    }

    public static void Room23()
    {
        WriteLines(new[] { TxtYouAreIn + TxtRoom23_01, TxtRoom23_02, TxtRoom23_03 });
        DescribeStreet();
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdPatrz + CmdPlakat)
            {
                Out.WriteLine(TxtRoom23_04);
                Out.WriteLine(TxtRoom23_05);
            }
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitZachod, TxtPlaceUlicaSklepikowa });
            if (MoveWhenOrQuit(new[] { Go(CmdZachod, RoomShopStreet) })) return;
            BakeryShop();
        } while (!(Session.Room != RoomBakery));
    }

    public static void Room24()
    {
        WriteLines(new[] { TxtYouAreIn + TxtRoom24_01, TxtRoom24_02, TxtRoom24_03 });
        DescribeStreet();
        do
        {
            ReadCommand();
            ArmouryShop();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceUlicaSklepikowa });
            if (MoveWhenOrQuit(new[] { Go(CmdWschod, RoomShopStreet) })) return;
            if (Command == CmdPatrz + CmdPlakat)
                Out.WriteLine(TxtRoom24_04);
            StreetFights();
        } while (!(Session.Room != RoomArmoury));
    }

    public static void Room25()
    {
        WriteLines(new[] { TxtRoom25_01, TxtRoom25_02 });
        DescribeStreet();
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitZachod, TxtPlaceUlicaSklepikowa });
            if (MoveWhenOrQuit(new[] { Go(CmdZachod, RoomShopStreet2) })) return;
            if (Command == CmdPatrz + CmdWywieszka)
                Out.WriteLine(TxtRoom25_03);
            StreetFights();
            GeneralStoreShop();
        } while (!(Session.Room != RoomGeneralStore));
    }

    public static void Room26()
    {
        Out.WriteLine(TxtRoom26_01);
        DescribeStreet();
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceUlicaSklepikowa });
            if (MoveWhenOrQuit(new[] { Go(CmdWschod, RoomShopStreet2) })) return;
            if (Command == CmdPatrz + CmdPlakat)
                Out.WriteLine(TxtRoom26_02);
            MagicShop();
            StreetFights();
        } while (!(Session.Room != RoomMagicShop));
    }

    public static void Room30()
    {
        BasicRoomSimple(RoomLongStreet,
                  new[] { TxtRoom30_01, TxtRoom30_02 },
                  new[] { TxtShared_03,
                   TxtExitWschod, TxtPlaceCentrumMiasta,
                   TxtExitZachod, TxtPlaceUlicaDluga,
                   TxtExitPolnoc, TxtPlaceWejscieNaArene },
                  new[] { Go(CmdWschod, RoomCityCentre),
                   Go(CmdZachod, RoomLongStreet2),
                   Go(CmdPolnoc, RoomArenaEntrance) });
    }

    public static void Room31()
    {
        BasicRoomQuit(RoomLongStreet2,
                  new[] { TxtRoom31_01, TxtRoom31_02 },
                  new[] { TxtShared_03,
                   TxtExitWschod, TxtPlaceUlicaDluga,
                   TxtExitPolnoc, TxtPlaceDolinaRozrywek },
                  new[] { Go(CmdWschod, RoomLongStreet), Go(CmdPolnoc, RoomValley) });
    }

    public static void Room32()
    {
        BasicRoomQuitPoster(RoomArenaEntrance,
                  new[] { TxtRoom32_01, TxtRoom32_02 },
                  new[] { TxtShared_03,
                   TxtExitPolodnie, TxtPlaceUlicaDluga,
                   TxtExitPolnoc, TxtPlaceArena },
                  new[] { Go(CmdPolodnie, RoomLongStreet), Go(CmdPolnoc, RoomArena33) },
                  new[] { TxtRoom32_03, TxtRoom32_04 });
    }

    public static void Room60()
    {
        BasicRoomSimple(RoomValley,
                  new[] { TxtYouAreIn + TxtRoom60_01, TxtRoom60_02, TxtRoom60_03 },
                  new[] { TxtShared_03,
                   TxtExitPolodnie, TxtPlaceUlicaDluga,
                   TxtExitWschod, TxtPlaceBudowlaPelnaHalasu },
                  new[] { Go(CmdPolodnie, RoomLongStreet2), Go(CmdWschod, RoomCrowd1) });
    }

    public static void Room75()
    {
        BasicRoomSimple(RoomDarkStreet,
                  TxtRoom75Lines1[1..],
                  new[] { TxtShared_03,
                   TxtExitZachod, TxtPlaceCentrumMiasta,
                   TxtExitPolnoc, TxtPlaceBarPodDwomaPedalami,
                   TxtExitPolodnie, TxtPlaceBluszcz },
                  new[] { Go(CmdZachod, RoomCityCentre), Go(CmdPolnoc, RoomBar),
                   Go(CmdPolodnie, RoomIvy1) });
    }

    public static void Room76()
    {
        Out.WriteLine(TxtYouAreIn + TxtRoom76_01);
        WriteLines(TxtRoom76Lines1[1..]);
        if (World.Monsters[(int)Maciek] == 76)
            Out.WriteLine(TxtRoom76_05);
        if (World.Monsters[(int)Pedal] == 76)
            Out.WriteLine(TxtRoom76_06);
        if (World.Monsters[(int)Couple] == 76)
            Out.WriteLine(TxtRoom76_07);
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlaceUlicaCiemna });
            if (MoveWhenOrQuit(new[] { Go(CmdPolodnie, RoomDarkStreet) })) return;
            if (Command == CmdLista)
                WriteLines(TxtRoom76Lines2[1..]);
            if (Command == CmdZabij + CmdPedal)
            {
                Fight(EasyEnemy);
                World.Monsters[(int)Pedal] = 0;
                PillGain();
            }
            if (Command == CmdZabij + CmdPara)
            {
                Fight(VeryHardEnemy);
                World.Monsters[(int)Couple] = 0;
                Fight(VeryHardEnemy);
            }
            if (Command == CmdZabij + CmdMaciek)
            {
                Fight(EasyEnemy);
                World.Monsters[(int)Maciek] = 0;
            }
            if (Command == CmdMam + Cmd18Lat)
                Out.WriteLine(TxtRoom76_13);
        } while (!(Session.Room != RoomBar));
    }

    public static void Room77()
    {
        WriteLines(new[] { TxtRoom77_01, TxtRoom77_02 });
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolnoc, TxtPlaceUlCiemna,
                            TxtExitWschod, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlaceBluszcz,
                            TxtExitPolodnie, TxtPlaceBluszcz });
            if (MoveWhenOrQuit(new[] { Go(CmdPolnoc, RoomDarkStreet),
                      Go(CmdWschod, RoomIvy2),
                      Go(CmdZachod, RoomIvy3),
                      Go(CmdPolodnie, RoomIvy4) })) return;
        } while (!(Session.Room != RoomIvy1));
    }

    public static void Room78()
    {
        WriteLines(new[] { TxtRoom78_01, TxtRoom78_02 });
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitZachod, TxtPlaceBluszcz });
            if (MoveWhenOrQuit(new[] { Go(CmdZachod, RoomIvy1) })) return;
        } while (!(Session.Room != RoomIvy2));
    }

    public static void Room79()
    {
        WriteLines(new[] { TxtRoom79_01, TxtRoom79_02 });
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceBluszcz });
            if (MoveWhenOrQuit(new[] { Go(CmdWschod, RoomIvy1) })) return;
        } while (!(Session.Room != RoomIvy3));
    }

    public static void Room80()
    {
        WriteLines(TxtRoom80Lines1[1..]);
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolnoc, TxtPlaceBluszcz,
                            TxtExitWschod, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlaceBluszcz,
                            TxtExitPolodnie, TxtPlaceBluszcz });
            if (MoveWhenOrQuit(new[] { Go(CmdPolnoc, RoomIvy1),
                      Go(CmdZachod, RoomIvy5),
                      Go(CmdWschod, RoomIvy6),
                      Go(CmdPolodnie, RoomClearing) })) return;
            if (Command == CmdPatrz + CmdNapis)
                Out.WriteLine(TxtRoom80_04);
            if (Command == CmdPatrz + CmdStudnia)
            {
                Combat.Roll = (short)(Roll(3) + 1);
                Out.WriteLine(TxtRoom80_05 + Combat.Roll + TxtShared_23);
                Player.Money = Player.Money + Combat.Roll;
                Combat.Roll = Roll(100);
                if (Combat.Roll < 10)
                {
                    Out.WriteLine(TxtRoom80_06);
                    Player.Energy = (short)(Player.Energy - 40);
                    Out.WriteLine(TxtRoom80_07);
                    Player.Money = Player.Money - 100;
                    if (Player.Money < 0) Player.Money = 0;
                }
            }
        } while (!(Session.Room != RoomIvy4));
    }

    public static void Room81()
    {
        WriteLines(new[] { TxtRoom81_01, TxtRoom81_02 });
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceBluszcz });
            if (MoveWhenOrQuit(new[] { Go(CmdWschod, RoomIvy4) })) return;
        } while (!(Session.Room != RoomIvy5));
    }

    public static void Room82()
    {
        WriteLines(new[] { TxtRoom82_01, TxtRoom82_02 });
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitZachod, TxtPlaceBluszcz });
            if (MoveWhenOrQuit(new[] { Go(CmdZachod, RoomIvy4) })) return;
        } while (!(Session.Room != RoomIvy6));
    }

    public static void Room83()
    {
        WriteLines(TxtRoom83Lines1[1..]);
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolnoc, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlacePodejrzaneKrzaki });
            if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomIvy4), Go(CmdZachod, RoomCavePath) })) return;
        } while (!(Session.Room != RoomClearing));
    }

    public static void Room84()
    {
        WriteLines(TxtRoom84Lines1[1..]);
        DescribePlants();
        do
        {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlaceBudynek,
                            TxtExitPolodnie, TxtPlaceWejscieDoGroty });
            if (MoveWhenOrQuit(new[] { Go(CmdWschod, RoomClearing), Go(CmdZachod, RoomLivingDoor), Go(CmdPolodnie, RoomCaveMouth) })) return;
        } while (!(Session.Room != RoomCavePath));
    }

    public static void Room85()
    {
        WriteLines(new[] { TxtRoom85_01, TxtRoom85_02 });
        if (World.Door != 0)
            Out.WriteLine(TxtRoom85_03);
        do
        {
            ReadCommand();
            ExpandAbbrev();
            if (Command == CmdZabij + CmdDrzwi)
            {
                if (World.Door != 0)
                {
                    Fight(NotEasyEnemy);
                    if (Player.Energy > 0) World.Door = 0;
                }
            }
            CheckMode();
            if (Command == CmdExit)
            {
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlacePodejrzaneKrzaki });
                if (World.Door != 0)
                    Out.WriteLine(TxtExitZachod + TxtPlaceGdybyNieTeCholerneDrzwi);
                else
                    Out.WriteLine(TxtExitZachod + TxtPlaceWejscieDoDomku);
            }
            if (MoveWhenOrQuit(new[] { Go(CmdWschod, RoomCavePath) })) return;
            if (Command == CmdZachod)
                if (World.Door == 0) Session.Room = RoomHut;
                else Out.WriteLine(TxtRoom85_07);
        } while (!(Session.Room != RoomLivingDoor));
    }

    public static void Room86()
    {
        Out.WriteLine(TxtRoom86_01);
        if (World.OldElf != 0)
            Out.WriteLine(TxtRoom86_02);
        if (World.OldElf == 0)
            Out.WriteLine(TxtRoom86_03);
        do
        {
            ReadCommand();
            ExpandAbbrev();
            if (Command == CmdDawaj + CmdKunszt) Player.Experience = (short)(Player.Experience + 500);
            if (Command == CmdPatrz + CmdGarnki)
                Out.WriteLine(TxtRoom86_04);
            if (Command == CmdRozmawiaj + CmdStaruch)
            {
                if ((Bag.Items[(int)TBagItem.Sausage] > -30) && (Player.Money < 200) && (Bag.Items[(int)TBagItem.ReturnScroll] == 0))
                    WriteLines(TxtRoom86Lines1[1..]);
                if ((Bag.Items[(int)TBagItem.Sausage] < -30) && (Player.Money > 199))
                {
                    WriteLines(TxtRoom86Lines2[1..]);
                    Session.Room = RoomCityCentre;
                    Bag.Items[(int)TBagItem.Beer] = (short)(Bag.Items[(int)TBagItem.Beer] - ItemUnit);
                    Bag.Items[(int)TBagItem.Sausage] = (short)(Bag.Items[(int)TBagItem.Sausage] + 40);
                    Player.Money = Player.Money - 200;
                    Bag.Items[(int)TBagItem.ReturnScroll] = (short)(Bag.Items[(int)TBagItem.ReturnScroll] - ItemUnit);
                    Player.Experience = (short)(Player.Experience + 50);
                    Bag.Count = (short)(Bag.Count - 2);
                }
                if (Bag.Items[(int)TBagItem.ReturnScroll] < 0)
                {
                    Out.WriteLine(TxtRoom86_13);
                    Session.Room = RoomCageHuge;
                }
            }
            if (Command == CmdZabij + CmdStaruch)
            {
                if (World.OldElf != 0)
                {
                    Fight(WeakEnemy);
                    World.OldElf = 0;
                    Out.WriteLine(TxtRoom86_14);
                    Out.WriteLine(TxtRoom86_15);
                    Player.Level = (sbyte)(Player.Level - 1);
                }
            }
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitWschod, TxtPlaceBudynek });
            if (MoveWhenOrQuit(new[] { Go(CmdWschod, RoomLivingDoor) })) return;
        } while (!(Session.Room != RoomHut));
    }

    public static void Room87()
    {
        Out.WriteLine(TxtRoom87_01);
        Out.WriteLine(TxtRoom87_02);
        Combat.Roll = Roll(100);
        if (Combat.Roll < 40)
        {
            Out.WriteLine(TxtRoom87_03 + (25 - Player.Stats[(int)TStat.Dexterity]) + TxtShared_26);
            Player.Energy = (short)(Player.Energy - 25 + Player.Stats[(int)TStat.Dexterity]);
        }
        do
        {
            ReadCommand();
            ExpandAbbrev();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlaceGrota,
                            TxtExitPolnoc, TxtPlacePodejrzaneKrzaki });
            if (MoveWhenOrQuit(new[] { Go(CmdPolnoc, RoomCavePath), Go(CmdPolodnie, RoomNettleThrone) })) return;
        } while (!(Session.Room != RoomCaveMouth));
    }

    public static void Room88()
    {
        WriteLines(TxtRoom88Lines1[1..]);
        Out.Write(TxtRoom88_05);
        Command = Crt.ReadLn();
        Out.WriteLine(TxtRoom88_06);
        do
        {
            Combat.EnemyEnergy = 200; Combat.EnemyDexterity = 15; Combat.EnemyStrength = 18;
            Combat.FireballCharges = 5; Combat.PoisonCharges = 1;
            DoFight();
            if (Combat.EnemyEnergy < 0)
            {
                LeafGain();
                PillGain();
                ScrollGain();
                if (World.DuncanQuest != 0) World.DuncanQuest = (sbyte)(World.DuncanQuest - 50);
            }
            if (Player.Energy > 0)
            {
                Out.WriteLine(TxtRoom88_07);
                Session.Room = RoomCaveMouth;
            }
            ReadCommand();
            ExpandAbbrev();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlaceWejscieDoGroty2 });
            if (MoveWhenOrQuit(new[] { Go(CmdPolodnie, RoomCaveMouth) })) return;
        } while (!(Session.Room != RoomNettleThrone));
    }

    public static void GrantQuestReward(short quest, short exp)
    {
        Out.WriteLine(TxtRoom100_12);
        Out.WriteLine(TxtQuestReward + exp + TxtSpaceKunsztDashes);
        Player.Experience = (short)(Player.Experience + exp);
        World.Quest = 0;
        World.QuestKills = 0;
        Bag.Permit = Bag.Permit - ItemUnit;
        switch (quest)
        {
            case 1: Bag.Count = (short)(Bag.Count + 1); break;
            case 2:
                Bag.Items[(int)TBagItem.Diploma] = 0;
                Player.MaxEnergy = (short)(Player.MaxEnergy - 5);
                break;
            case 3:
                Bag.Count = (short)(Bag.Count - 1);
                Player.Stats[(int)TStat.Wisdom]--;
                Bag.Items[(int)TBagItem.Pipe] = (short)(Bag.Items[(int)TBagItem.Pipe] + ItemUnit);
                Player.Practice = (short)(Player.Practice - 1);
                break;
        }
    }

    public static void Room100()
    {
        WriteLines(new[] { TxtRoom100_01, TxtRoom100_02 });
        DescribeStreet();
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
            {
                WriteExits(new[] { TxtShared_03,
                            TxtExitPolodnie, TxtPlaceUlSklepikowa,
                            TxtExitWschod, TxtPlaceDrogaOKtorejMozeszPomazycBoJestWProdukcji });
                if (Bag.Permit == 0) Out.WriteLine(TxtExitZachod + TxtPlaceDrogaZablokowanaPrzezQuestMastera);
                if (Bag.Permit < 0) Out.WriteLine(TxtExitZachod + TxtPlaceDroga);
            }
            if (Command == CmdLista)
                WriteLines(TxtRoom100Lines1[1..]);
            for (int i = 1; i < QuestOffers.Length; i++)
            {
                TQuestOffer q = QuestOffers[i];
                BuyQuest(q.cmd, q.line, q.quest, q.kills, q.price);
            }
            if ((Command == CmdSprzedaj + CmdQuest) && (World.Quest > 0))
            {
                switch (World.Quest)
                {
                    case 1: if (World.QuestKills < 1) GrantQuestReward(1, 100); break;
                    case 2: if ((World.QuestKills < 1) && (Bag.Items[(int)TBagItem.Diploma] <= -ItemUnit)) GrantQuestReward(2, 250); break;
                    case 3: if ((World.QuestKills < 1) && (Bag.Items[(int)TBagItem.Pipe] <= -ItemUnit) && (Player.Practice > 0)) GrantQuestReward(3, 425); break;
                }
            }
            if (MoveWhenOrExit(new[] { Go(CmdPolodnie, RoomShopStreet2) })) return;
            if ((Command == CmdZachod) && (Bag.Permit <= -ItemUnit)) Session.Room = RoomRoad1;
            if ((Command == CmdZachod) && (Bag.Permit == 0)) Out.WriteLine(TxtRoom100_16);
            if (Command == CmdZabij + CmdQuestMaster)
            {
                Combat.PoisonCharges = 10;
                Combat.FireballCharges = 20;
                Combat.EnemyStrength = 10;
                Combat.EnemyDexterity = 26;
                Combat.EnemyEnergy = 188;
                DoFight();
                if (Combat.EnemyEnergy < 1) Bag.Permit = Bag.Permit - ItemUnit;
            }
        } while (!(Session.Room != RoomQuestMaster));
    }

    public static void Room101()
    {
        BasicRoomQuitGeneric(RoomRoad1,
                  TxtRoom101Lines1[1..],
                  new[] { TxtShared_03,
                   TxtExitWschod, TxtPlaceRozjazd,
                   TxtExitZachod, TxtPlaceDroga,
                   TxtExitPolnoc, TxtPlaceLas,
                   TxtExitPolodnie, TxtPlaceLas },
                  new[] { Go(CmdWschod, RoomQuestMaster), Go(CmdZachod, RoomRoad2),
                   Go(CmdPolnoc, RoomForest) },
                  CmdPatrz + CmdDrogowskaz,
                  new[] { TxtRoom101_05 });
    }

    public static void Room102()
    {
        BasicRoomQuit(RoomRoad2,
                  new[] { TxtRoom102_01, TxtRoom102_02 },
                  new[] { TxtShared_03,
                   TxtExitWschod, TxtPlaceDroga,
                   TxtExitZachod, TxtPlaceDroga,
                   TxtExitPolnoc, TxtPlaceLas,
                   TxtExitPolodnie, TxtPlaceLas },
                  new[] { Go(CmdWschod, RoomRoad1), Go(CmdZachod, RoomRoad3) });
    }

    public static void Room103()
    {
        BasicRoomQuit(RoomRoad3,
                  TxtRoom103Lines1[1..],
                  new[] { TxtShared_03,
                   TxtExitWschod, TxtPlaceDroga,
                   TxtExitPolnoc, TxtPlaceLas,
                   TxtExitPolodnie, TxtPlaceLas },
                  new[] { Go(CmdWschod, RoomRoad2) });
    }

    public static void ConcertRooms()
    {
        if (Session.Room == RoomCrowd1)
        {
            WriteLines(TxtConcertRoomsLines1[1..]);
            DescribeCrowd();
            do
            {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitZachod, TxtPlaceDolinaRozrywek,
                                TxtExitPolnoc, TxtPlaceTlum,
                                TxtExitPolodnie, TxtPlaceTlum });
                ConcertFights();
                if (MoveWhenOrExit(new[] { Go(CmdZachod, RoomValley),
                          Go(CmdPolnoc, RoomCrowd3),
                          Go(CmdPolodnie, RoomCrowd2),
                          Go(CmdWschod, RoomCrowd4) })) return;
                if (Command == CmdPatrz + CmdPlakat)
                {
                    Out.WriteLine(TxtConcertRooms_07);
                    Out.WriteLine(TxtConcertRooms_08);
                }
            } while (!(Session.Room != RoomCrowd1));
        }
        if (Session.Room == RoomCrowd2)
        {
            Out.WriteLine(TxtConcertRooms_09);
            DescribeCrowd();
            do
            {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitPolnoc, TxtPlaceTlum,
                                TxtExitWschod, TxtPlaceTlum });
                ConcertFights();
                if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomCrowd1), Go(CmdWschod, RoomCrowd5) })) return;
            } while (!(Session.Room != RoomCrowd2));
        }
        if (Session.Room == RoomCrowd3)
        {
            Out.WriteLine(TxtConcertRooms_11);
            DescribeCrowd();
            do
            {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitPolodnie, TxtPlaceTlum,
                                TxtExitWschod, TxtPlaceTlum,
                                TxtExitPolnoc, TxtPlacePiwiarnia });
                ConcertFights();
                if (MoveWhenOrExit(new[] { Go(CmdPolodnie, RoomCrowd1), Go(CmdWschod, RoomCrowd6), Go(CmdPolnoc, RoomPub) })) return;
            } while (!(Session.Room != RoomCrowd3));
        }
        if (Session.Room == RoomCrowd4)
        {
            Out.WriteLine(TxtYouAreIn + TxtConcertRooms_13);
            Out.WriteLine(TxtConcertRooms_14);
            DescribeCrowd();
            do
            {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitZachod, TxtPlaceTlum,
                                TxtExitPolnoc, TxtPlaceTlum,
                                TxtExitPolodnie, TxtPlaceTlum });
                ConcertFights();
                if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomCrowd6), Go(CmdZachod, RoomCrowd1), Go(CmdPolodnie, RoomCrowd5) })) return;
            } while (!(Session.Room != RoomCrowd4));
        }
        if (Session.Room == RoomCrowd5)
        {
            Out.WriteLine(TxtConcertRooms_16);
            Out.WriteLine(TxtConcertRooms_14);
            DescribeCrowd();
            do
            {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitZachod, TxtPlaceTlum,
                                TxtExitPolnoc, TxtPlaceTlum });
                ConcertFights();
                if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomCrowd4), Go(CmdZachod, RoomCrowd2) })) return;
            } while (!(Session.Room != RoomCrowd5));
        }
        if (Session.Room == RoomCrowd6)
        {
            Out.WriteLine(TxtConcertRooms_17);
            DescribeCrowd();
            do
            {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitZachod, TxtPlaceTlum,
                                TxtExitPolodnie, TxtPlaceTlum });
                ConcertFights();
                if (MoveWhenOrExit(new[] { Go(CmdPolodnie, RoomCrowd4), Go(CmdZachod, RoomCrowd3) })) return;
            } while (!(Session.Room != RoomCrowd6));
        }
        if (Session.Room == RoomPub)
        {
            Out.WriteLine(TxtYouAreIn + TxtConcertRooms_18);
            Out.WriteLine(TxtConcertRooms_19);
            if (World.Monsters[(int)Minibarman] == 67)
                Out.WriteLine(TxtConcertRooms_20);
            if (World.Monsters[(int)Fatso] == 67)
                Out.WriteLine(TxtConcertRooms_21);
            DescribeCrowd();
            do
            {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitWschod, TxtPlaceWejscieNaEstrade,
                                TxtExitPolodnie, TxtPlaceTlum });
                ConcertFights();
                if ((Command == CmdZabij + CmdMiniBarman) && (World.Monsters[(int)Minibarman] == 67))
                {
                    Out.WriteLine(TxtConcertRooms_23);
                    Out.WriteLine(TxtConcertRooms_24);
                    Fight(VeryHardEnemy);
                    if (Player.Energy > 0)
                    {
                        World.Monsters[(int)Minibarman] = 0;
                        Combat.Roll = Roll(100);
                        if (Combat.Roll < 15)
                        {
                            Out.WriteLine(TxtConcertRooms_25 + TxtShared_08 + TxtPiwoTail);
                            Bag.Items[(int)TBagItem.Beer] = (short)(Bag.Items[(int)TBagItem.Beer] - ItemUnit);
                            Bag.Count = (short)(Bag.Count + 1);
                        }
                    }
                }
                if ((Command == CmdZabij + CmdGrubas) && (World.Monsters[(int)Fatso] == 67))
                {
                    Fight(VeryHardEnemy);
                    if (Player.Energy > 0)
                    {
                        World.Monsters[(int)Fatso] = 0;
                        if (Combat.Roll < 15)
                        {
                            Out.WriteLine(TxtConcertRooms_26 + TxtShared_08 + TxtPiwoTail);
                            Bag.Items[(int)TBagItem.Beer] = (short)(Bag.Items[(int)TBagItem.Beer] - ItemUnit);
                            Bag.Count = (short)(Bag.Count + 1);
                        }
                    }
                }
                if (MoveWhenOrExit(new[] { Go(CmdPolodnie, RoomCrowd3), Go(CmdWschod, RoomStageEntrance) })) return;
            } while (!(Session.Room != RoomPub));
        }
        if (Session.Room == RoomStageEntrance)
        {
            Out.WriteLine(TxtConcertRooms_27);
            Out.WriteLine(TxtConcertRooms_28);
            DescribeCrowd();
            do
            {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                {
                    WriteExits(new[] { TxtShared_03,
                                TxtExitZachod, TxtPlacePiwiarnia,
                                TxtExitWschod, TxtPlaceScena });
                    Out.WriteLine(TxtConcertRooms_31);
                }
                ConcertFights();
                if (MoveWhenOrExit(new[] { Go(CmdZachod, RoomPub), Go(CmdWschod, RoomBackstage) })) return;
                if (Command == CmdPatrz + CmdTabliczka)
                    Out.WriteLine(TxtConcertRooms_32);
            } while (!(Session.Room != RoomStageEntrance));
        }
        if (Session.Room == RoomBackstage)
        {
            Out.WriteLine(TxtConcertRooms_33);
            Out.WriteLine(TxtConcertRooms_34);
            if (World.Monsters[(int)DJ] == 69)
                Out.WriteLine(TxtConcertRooms_35);
            do
            {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitZachod, TxtPlaceWejscieNaScene,
                                TxtExitPolodnie, TxtPlaceScena });
                if (Command == CmdZabij + CmdDJ)
                {
                    if (World.Monsters[(int)DJ] == 69)
                    {
                        Fight(VeryHardEnemy);
                        if (Combat.EnemyEnergy < 0)
                        {
                            World.Monsters[(int)DJ] = 0;
                            Combat.Roll = Roll(100);
                            if (Combat.Roll < 25)
                            {
                                Out.WriteLine(TxtShared_08 + TxtConcertRooms_38);
                                Bag.Items[(int)TBagItem.DryRation] = (short)(Bag.Items[(int)TBagItem.DryRation] - ItemUnit);
                                Bag.Count = (short)(Bag.Count + 1);
                            }
                            if (Combat.Roll < 3)
                            {
                                Out.WriteLine(TxtShared_08 + TxtConcertRooms_39);
                                Out.WriteLine(TxtBang + TxtUniqeItem + TxtConcertRooms_40);
                                Bag.Items[(int)TBagItem.Cassette] = (short)(Bag.Items[(int)TBagItem.Cassette] - ItemUnit);
                                Player.MaxEnergy = (short)(Player.MaxEnergy + 5);
                                Gear.Values[(int)TGearItem.Armor] = (short)(Gear.Values[(int)TGearItem.Armor] - 8);
                                Bag.Count = (short)(Bag.Count + 1);
                                Player.Stats[(int)TStat.Dexterity]++;
                            }
                        }
                    }
                }
                if (MoveWhenOrExit(new[] { Go(CmdZachod, RoomStageEntrance), Go(CmdPolodnie, RoomStage1) })) return;
            } while (!(Session.Room != RoomBackstage));
        }
        if (Session.Room == RoomStage1)
        {
            WriteLines(TxtConcertRoomsLines2[1..]);
            DescribeStage();
            do
            {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitPolnoc, TxtPlaceScena,
                                TxtExitPolodnie, TxtPlaceSrodekSceny });
                if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomBackstage), Go(CmdPolodnie, RoomStage2) })) return;
                StageFights();
            } while (!(Session.Room != RoomStage1));
        }
        if (Session.Room == RoomStage2)
        {
            Out.WriteLine(TxtConcertRooms_46);
            Out.WriteLine(TxtConcertRooms_47);
            DescribeStage();
            do
            {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitPolnoc, TxtPlaceScena,
                                TxtExitPolodnie, TxtPlaceScena });
                if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomStage1), Go(CmdPolodnie, RoomStage3) })) return;
                StageFights();
            } while (!(Session.Room != RoomStage2));
        }
        if (Session.Room == RoomStage3)
        {
            Out.WriteLine(TxtConcertRooms_48);
            Out.WriteLine(TxtConcertRooms_49);
            DescribeStage();
            do
            {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits(new[] { TxtShared_03,
                                TxtExitPolnoc, TxtPlaceSrodekSceny });
                if (MoveWhenOrExit(new[] { Go(CmdPolnoc, RoomStage2) })) return;
                StageFights();
            } while (!(Session.Room != RoomStage3));
        }
    }

    public static void PlainRooms()
    {
        for (int i = 1; i < SideRooms.Length; i++)
        {
            TSideRoom s = SideRooms[i];
            SimpleRoom(s.room, s.dest, s.direction, s.exitDesc);
        }
    }

    public static void BasicRoomGeneric(short RoomNo, string[] intro, string[] exits,
        TMovement[] moves, string posterCmd, string[] poster)
    {
        if (Session.Room != RoomNo) return;
        WriteLines(intro);
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(exits);
            if (MoveWhenOrExit(moves)) return;
            if ((poster.Length > 0) && (Command == posterCmd))
                WriteLines(poster);
        } while (!(Session.Room != RoomNo));
    }

    public static void BasicRoomPoster(short RoomNo, string[] intro, string[] exits,
        TMovement[] moves, string[] poster)
    {
        BasicRoomGeneric(RoomNo, intro, exits, moves, CmdPatrz + CmdPlakat, poster);
    }

    public static void BasicRoomQuit(short RoomNo, string[] intro, string[] exits,
        TMovement[] moves)
    {
        BasicRoomQuitGeneric(RoomNo, intro, exits, moves, "", Array.Empty<string>());
    }

    public static void BasicRoomQuitGeneric(short RoomNo, string[] intro, string[] exits,
        TMovement[] moves, string posterCmd, string[] poster)
    {
        if (Session.Room != RoomNo) return;
        WriteLines(intro);
        do
        {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(exits);
            if (MoveWhenOrQuit(moves)) return;
            if ((poster.Length > 0) && (Command == posterCmd))
                WriteLines(poster);
        } while (!(Session.Room != RoomNo));
    }

    public static void BasicRoomQuitPoster(short RoomNo, string[] intro, string[] exits,
        TMovement[] moves, string[] poster)
    {
        BasicRoomQuitGeneric(RoomNo, intro, exits, moves, CmdPatrz + CmdPlakat, poster);
    }

    public static void BasicRoomSimple(short RoomNo, string[] intro, string[] exits,
        TMovement[] moves)
    {
        BasicRoomGeneric(RoomNo, intro, exits, moves, "", Array.Empty<string>());
    }

    public static void SimpleRoom(short RoomNo, short DestRoom, string Direction, string ExitDesc)
    {
        if (Session.Room == RoomNo)
        {
            Out.WriteLine(TxtYouAreIn + TxtSimpleRoom_01);
            do
            {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                {
                    Out.WriteLine(TxtShared_03);
                    Out.WriteLine(Direction + TxtSimpleRoom_02 + ExitDesc);
                }
                if (MoveWhenOrExit(new[] { Go(Direction, DestRoom) })) return;
            } while (!(Session.Room != RoomNo));
        }
    }

    public sealed class TRoomEntry
    {
        public short id;
        public Action run;
        public TRoomEntry(short id, Action run) { this.id = id; this.run = run; }
    }

    public static readonly TRoomEntry[] RoomDispatch = { null!,
        new(RoomStart, Room0),
        new(RoomSalon, Room1),
        new(RoomTraining, Room2),
        new(RoomMeditation, Room3),
        new(RoomSchoolHall, Room4),
        new(RoomLost, Room5),
        new(RoomAny, PlainRooms),
        new(RoomSchoolLanding, Room9),
        new(RoomCageCorridor, Room11),
        new(RoomCageWeakling, Room12),
        new(RoomCageStrong, Room13),
        new(RoomCageAgile, Room14),
        new(RoomCageTough, Room15),
        new(RoomCageHuge, Room16),
        new(RoomCageReward, Room17),
        new(RoomTeleport, Room18),
        new(RoomCityCentre, Room20),
        new(RoomDarkStreet, Room75),
        new(RoomLongStreet, Room30),
        new(RoomBar, Room76),
        new(RoomIvy1, Room77),
        new(RoomIvy2, Room78),
        new(RoomIvy3, Room79),
        new(RoomIvy4, Room80),
        new(RoomIvy5, Room81),
        new(RoomIvy6, Room82),
        new(RoomClearing, Room83),
        new(RoomCavePath, Room84),
        new(RoomCaveMouth, Room87),
        new(RoomNettleThrone, Room88),
        new(RoomLivingDoor, Room85),
        new(RoomHut, Room86),
        new(RoomShopStreet, Room21),
        new(RoomShopStreet2, Room22),
        new(RoomRoad1, Room101),
        new(RoomRoad2, Room102),
        new(RoomRoad3, Room103),
        new(RoomQuestMaster, Room100),
        new(RoomBakery, Room23),
        new(RoomArmoury, Room24),
        new(RoomGeneralStore, Room25),
        new(RoomMagicShop, Room26),
        new(RoomLongStreet2, Room31),
        new(RoomValley, Room60),
        new(RoomAny, ConcertRooms),
        new(RoomArenaEntrance, Room32),
    };

    public static void DispatchRoom()
    {
        for (int ri = 1; ri < RoomDispatch.Length; ri++)
            if ((RoomDispatch[ri].id == -1) || (Session.Room == RoomDispatch[ri].id))
            {
                RoomDispatch[ri].run();
                if (Session.ExitRequested) return;
            }
    }
}
