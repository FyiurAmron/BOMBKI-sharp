
using static Game;
using static Game.TMonster;
using static Game.TFoe;
using static Game.TStreetKind;
using static Texts;

public static class Rooms {
    public const int RoomAny = -1;
    public const int RoomStart = 0;
    public const int RoomSalon = 1;
    public const int RoomTraining = 2;
    public const int RoomMeditation = 3;
    public const int RoomSchoolHall = 4;
    public const int RoomLost = 5;
    public const int RoomSchoolSide1 = 6;
    public const int RoomSchoolSide2 = 7;
    public const int RoomSchoolSide3 = 8;
    public const int RoomSchoolLanding = 9;
    public const int RoomSchoolSide4 = 10;
    public const int RoomCageCorridor = 11;
    public const int RoomCageWeakling = 12;
    public const int RoomCageStrong = 13;
    public const int RoomCageAgile = 14;
    public const int RoomCageTough = 15;
    public const int RoomCageHuge = 16;
    public const int RoomCageReward = 17;
    public const int RoomTeleport = 18;
    public const int RoomCityCentre = 20;
    public const int RoomShopStreet = 21;
    public const int RoomShopStreet2 = 22;
    public const int RoomBakery = 23;
    public const int RoomArmoury = 24;
    public const int RoomGeneralStore = 25;
    public const int RoomMagicShop = 26;
    public const int RoomLongStreet = 30;
    public const int RoomLongStreet2 = 31;
    public const int RoomArenaEntrance = 32;
    public const int RoomArena33 = 33;
    public const int RoomArena34 = 34;
    public const int RoomArena35 = 35;
    public const int RoomArena36 = 36;
    public const int RoomArena37 = 37;
    public const int RoomArena38 = 38;
    public const int RoomArena39 = 39;
    public const int RoomArena40 = 40;
    public const int RoomArena41 = 41;
    public const int RoomArena42 = 42;
    public const int RoomArena43 = 43;
    public const int RoomArena44 = 44;
    public const int RoomArena45 = 45;
    public const int RoomArena46 = 46;
    public const int RoomArena47 = 47;
    public const int RoomArena48 = 48;
    public const int RoomArena49 = 49;
    public const int RoomArena50 = 50;
    public const int RoomArena51 = 51;
    public const int RoomArena52 = 52;
    public const int RoomArena53 = 53;
    public const int RoomArena54 = 54;
    public const int RoomArena55 = 55;
    public const int RoomArena56 = 56;
    public const int RoomArena57 = 57;
    public const int RoomValley = 60;
    public const int RoomCrowd1 = 61;
    public const int RoomCrowd2 = 62;
    public const int RoomCrowd3 = 63;
    public const int RoomCrowd4 = 64;
    public const int RoomCrowd5 = 65;
    public const int RoomCrowd6 = 66;
    public const int RoomPub = 67;
    public const int RoomStageEntrance = 68;
    public const int RoomBackstage = 69;
    public const int RoomStage1 = 70;
    public const int RoomStage2 = 71;
    public const int RoomStage3 = 72;
    public const int RoomDarkStreet = 75;
    public const int RoomBar = 76;
    public const int RoomIvy1 = 77;
    public const int RoomIvy2 = 78;
    public const int RoomIvy3 = 79;
    public const int RoomIvy4 = 80;
    public const int RoomIvy5 = 81;
    public const int RoomIvy6 = 82;
    public const int RoomClearing = 83;
    public const int RoomCavePath = 84;
    public const int RoomLivingDoor = 85;
    public const int RoomHut = 86;
    public const int RoomCaveMouth = 87;
    public const int RoomNettleThrone = 88;
    public const int RoomQuestMaster = 100;
    public const int RoomRoad1 = 101;
    public const int RoomRoad2 = 102;
    public const int RoomRoad3 = 103;
    public const int RoomForest = 104;
    public const int RoomQuit = 193;
    public const int TranceRoom = 1000;
    public const int DeadRoom = 10000;

    public static void Room0() {
        Out.WriteLine(TxtRoom0_01);
        do {
            ReadCommand();
            if (Command == CmdExit) Out.WriteLine(TxtRoom0_02);
            if (MoveWhenOrExit([Go(CmdPolnoc, RoomSalon)])) return;
        } while (!(Session.Room != RoomStart));
    }

    public static void Room1() {
        BasicRoomPoster(RoomSalon,
                  [ TxtYouAreIn + TxtRoom1_01, TxtRoom1Lines1[1], TxtRoom1Lines1[2],
                   TxtRoom1Lines1[3], ],
                  [ TxtShared_03,
                   TxtExitPolodnie, TxtPlaceTamGdzieZaczynaszGre,
                   TxtExitPolnoc, TxtPlaceMiasto,
                   TxtExitWschod, TxtPlacePokojTreningowy1,
                   TxtExitZachod, TxtPlaceMudSzkola,
                   TxtExitDol, TxtPlacePoddziemnyPokojTreningowy2, ],
                  [ Go(CmdPolodnie, RoomStart),
                   Go(CmdWschod, RoomTraining),
                   Go(CmdDol, RoomMeditation),
                   Go(CmdZachod, RoomSchoolHall),
                   Go(CmdPolnoc, RoomCityCentre), ],
                  TxtRoom1Lines2.Skip(1).ToList());
    }

    public static void Room2() {
        WriteLines([TxtYouAreIn + TxtRoom2_01, TxtRoom2_02, TxtShared_04]);
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitZachod, TxtPlacePok224JCentralny, ]);
            if (MoveWhenOrExit([Go(CmdZachod, RoomSalon)])) return;
            if (Command == CmdPatrz + CmdPlakat)
                WriteLines(TxtRoom2Lines1.Skip(1).ToList());
            HandleTrain();
        } while (!(Session.Room != RoomTraining));
    }

    public static void Room3() {
        WriteLines(TxtRoom3Lines1.Skip(1).ToList());
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                Out.WriteLine(TxtRoom3_04);
            if (MoveWhenOrExit([Go(CmdGora, RoomSalon)])) return;
            Training();
        } while (!(Session.Room != RoomMeditation));
    }

    public static void Room4() {
        BasicRoomGeneric(RoomSchoolHall,
                  [TxtYouAreIn + TxtRoom4_01, TxtRoom4_02],
                  [ TxtShared_03,
                   TxtExitWschod, TxtPlacePokojCentralny,
                   TxtExitZachod, TxtPlaceMudSzkola2, ],
                  [Go(CmdWschod, RoomSalon), Go(CmdZachod, RoomLost)],
                  CmdPatrz + CmdAfisz,
                  TxtRoom4Lines1.Skip(1).ToList());
    }

    public static void Room5() {
        BasicRoomPoster(RoomLost,
                  TxtRoom5Lines1.Skip(1).ToList(),
                  [ TxtShared_03,
                   TxtExitWschod, TxtPlaceMudSzkola,
                   TxtExitZachod, TxtPlacePokoj,
                   TxtExitPolnoc, TxtPlacePokoj,
                   TxtExitPolodnie, TxtPlacePokoj,
                   TxtExitGora, TxtPlacePokoj,
                   TxtExitDol, TxtPlacePokoj, ],
                  [ Go(CmdWschod, RoomSchoolHall),
                   Go(CmdZachod, RoomSchoolSide1),
                   Go(CmdPolnoc, RoomSchoolSide2),
                   Go(CmdPolodnie, RoomSchoolSide3),
                   Go(CmdGora, RoomSchoolLanding),
                   Go(CmdDol, RoomSchoolSide4), ],
                  [TxtRoom5_10, TxtRoom5_11]);
    }

    public static void Room9() {
        BasicRoomSimple(RoomSchoolLanding,
                  [TxtYouAreIn + TxtRoom9_01, TxtRoom9_02],
                  [ TxtShared_03,
                   TxtExitDol, TxtPlaceMudSzkola2,
                   TxtExitGora, TxtPlaceKlatkiPelneGajdy, ],
                  [Go(CmdDol, RoomLost), Go(CmdGora, RoomCageCorridor)]);
    }

    public static void Room11() {
        Out.WriteLine(TxtYouAreIn + TxtRoom11_01);
        WriteLines(TxtRoom11Lines1.Skip(1).ToList());
        do {
            ReadCommand();
            ExpandAbbrev();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceKlatkaCieniasa,
                            TxtExitZachod, TxtPlaceKlatkaSilnego,
                            TxtExitPolnoc, TxtPlaceKlatkaZrecznego,
                            TxtExitPolodnie, TxtPlaceKlatkaOdpornego,
                            TxtExitGora, TxtPlaceKlatkaSilnegoOdpornegoIZrecznego,
                            TxtExitDol, TxtPlaceJedynaDrogaNieProwadzacaDoKlatek, ]);
            if (MoveWhenOrExit([ Go(CmdWschod, RoomCageWeakling),
                      Go(CmdZachod, RoomCageStrong),
                      Go(CmdPolnoc, RoomCageAgile),
                      Go(CmdPolodnie, RoomCageTough),
                      Go(CmdGora, RoomCageHuge),
                      Go(CmdDol, RoomCageReward), ]))
                return;
            if (Command == CmdPatrz + CmdPlakat)
                WriteLines(TxtRoom11Lines2.Skip(1).ToList());
        } while (!(Session.Room != RoomCageCorridor));
    }

    public static void CageMonsterFight(IntSlot monster, short room, short energy, short dexterity, short strength, short lootRand, short lootThreshold) {
        if ((Command == CmdZabij + CmdPotwor) && (monster.Get() == room)) {
            Combat.EnemyEnergy = energy;
            Combat.EnemyDexterity = dexterity;
            Combat.EnemyStrength = strength;
            DoFight();
            if (Combat.Fled == 0) {
                monster.Set(0);
                Combat.Loot = Roll(lootRand);
                Out.WriteLine(TxtShared_08 + Combat.Loot + TxtShared_09);
                Player.Money = Player.Money + Combat.Loot;
                TryCageLoot(lootThreshold);
            } else
                Combat.Fled = 0;
        }
    }

    public static void DescribeCage(TMonster monster, short room, string aliveLine, string deadLine) {
        Out.WriteLine(TxtShared_06);
        if (World.Monsters[(int)monster] == room) Out.WriteLine(aliveLine);
        else if (World.Monsters[(int)monster] == 0) Out.WriteLine(deadLine);
    }

    public static void Room12() {
        DescribeCage(CageWeakling, RoomCageWeakling, TxtRoom12_01, TxtShared_07);
        do {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitZachod, TxtPlaceKlatkiPelneGajdy, ]);
            CheckMode();
            if (MoveWhenOrExit([Go(CmdZachod, RoomCageCorridor)])) return;
            CageMonsterFight(IntSlot.Into(World.Monsters, (int)CageWeakling), RoomCageWeakling, 20, 3, 3, 10, 4);
        } while (!(Session.Room != RoomCageWeakling));
    }

    public static void Room13() {
        DescribeCage(CageStrong, RoomCageStrong, TxtRoom13_01, TxtShared_07);
        do {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceKlatkiPelneGajdy, ]);
            CheckMode();
            if (MoveWhenOrExit([Go(CmdWschod, RoomCageCorridor)])) return;
            CageMonsterFight(IntSlot.Into(World.Monsters, (int)CageStrong), RoomCageStrong, 20, 3, 10, 15, 7);
        } while (!(Session.Room != RoomCageStrong));
    }

    public static void Room14() {
        DescribeCage(CageAgile, RoomCageAgile, TxtRoom14_01, TxtRoom14_02);
        do {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlaceKlatkiPelneGajdy, ]);
            CheckMode();
            if (MoveWhenOrQuit([Go(CmdPolodnie, RoomCageCorridor)])) return;
            CageMonsterFight(IntSlot.Into(World.Monsters, (int)CageAgile), RoomCageAgile, 20, 30, 3, 15, 7);
        } while (!(Session.Room != RoomCageAgile));
    }

    public static void Room15() {
        DescribeCage(CageTough, RoomCageTough, TxtRoom15_01, TxtShared_07);
        do {
            ReadCommand();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolnoc, TxtPlaceKlatkiPelneGajdy, ]);
            CheckMode();
            if (MoveWhenOrQuit([Go(CmdPolnoc, RoomCageCorridor)])) return;
            CageMonsterFight(IntSlot.Into(World.Monsters, (int)CageTough), RoomCageTough, 40, 3, 3, 15, 7);
        } while (!(Session.Room != RoomCageTough));
    }

    public static void Room16() {
        DescribeCage(CageHuge, RoomCageHuge, TxtRoom16_01, TxtShared_07);
        do {
            if (World.Monsters[(int)CageHuge] == RoomCageHuge) {
                Combat.EnemyEnergy = 40;
                Combat.EnemyDexterity = 11;
                Combat.EnemyStrength = 10;
                DoFight();
                if (Combat.Fled != 0) {
                    Combat.Fled = 0;
                    Session.Room = RoomCageCorridor;
                } else {
                    World.Monsters[(int)CageHuge] = 0;
                    Combat.Loot = Roll(30);
                    Out.WriteLine(TxtShared_08 + Combat.Loot + TxtShared_09);
                    Player.Money = Player.Money + Combat.Loot;
                    TryCageLoot(10);
                }
            }
            ReadCommand();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitDol, TxtPlaceKlatkiPelneGajdy, ]);
            CheckMode();
            if (MoveWhenOrQuit([Go(CmdDol, RoomCageCorridor)])) return;
        } while (!(Session.Room != RoomCageHuge));
    }

    public static void Room17() {
        Out.WriteLine(TxtYouAreIn + TxtRoom17_01);
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitGora, TxtPlaceKlatkiPelneGajdy,
                            TxtExitDol, TxtPlaceTeleport, ]);
            if (Command == CmdPatrz + CmdPlakat) {
                WriteLines(TxtRoom17Lines1.Skip(1).ToList());
                if ((World.Monsters[(int)CageWeakling] == 0) && (World.Monsters[(int)CageAgile] == 0) &&
                (World.Monsters[(int)CageStrong] == 0) && (World.Monsters[(int)CageTough] == 0) &&
                (World.Monsters[(int)CageHuge] == 0) && (Bag.Items[(int)TBagItem.Diploma] == 0))
                    Bag.Items[(int)TBagItem.Diploma] = RoomCageReward;
            }
            if (MoveWhenOrQuit([Go(CmdGora, RoomCageCorridor), Go(CmdDol, RoomTeleport)])) return;
        } while (!(Session.Room != RoomCageReward));
    }

    public static void Room18() {
        WriteLines(TxtRoom18Lines1.Skip(1).ToList());
        Session.Room = RoomSalon;
    }

    public static void Room20() {
        Out.WriteLine(TxtYouAreIn + TxtRoom20_01);
        DescribeStreet();
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdPatrz + CmdPlakat)
                Out.WriteLine(TxtRoom20_02);
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlacePokojCentralny,
                            TxtExitPolnoc, TxtPlaceUlSklepikowa,
                            TxtExitWschod, TxtPlaceUlCiemna,
                            TxtExitZachod, TxtPlaceUlDluga, ]);
            if (Command == CmdWyjsc0ie) {
                Session.ExitRequested = true;
                return;
            }
            MoveWhen([ Go(CmdPolodnie, RoomSalon),
                      Go(CmdPolnoc, RoomShopStreet),
                      Go(CmdZachod, RoomLongStreet),
                      Go(CmdWschod, RoomDarkStreet), ]);
            StreetFights();
        } while (!(Session.Room != RoomCityCentre));
    }

    public static void Room21() {
        Out.WriteLine(TxtRoom21_01);
        Out.WriteLine(TxtRoom21_02);
        DescribeStreet();
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlaceCentrumMiasta,
                            TxtExitPolnoc, TxtPlaceUlSklepikowa,
                            TxtExitWschod, TxtPlacePiekarnia,
                            TxtExitZachod, TxtPlaceZbrojownia, ]);
            if (MoveWhenOrQuit([ Go(CmdPolodnie, RoomCityCentre),
                      Go(CmdPolnoc, RoomShopStreet2),
                      Go(CmdWschod, RoomBakery),
                      Go(CmdZachod, RoomArmoury), ]))
                return;
            StreetFights();
        } while (!(Session.Room != RoomShopStreet));
    }

    public static void Room22() {
        Out.WriteLine(TxtRoom22_01);
        Out.WriteLine(TxtRoom22_02);
        DescribeStreet();
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlaceUlSklepikowa,
                            TxtExitPolnoc, TxtPlaceRozjazd,
                            TxtExitWschod, TxtPlaceSklepWielobranzowy,
                            TxtExitZachod, TxtPlaceSklepMagiczny, ]);
            if (MoveWhenOrQuit([ Go(CmdPolodnie, RoomShopStreet),
                      Go(CmdWschod, RoomGeneralStore),
                      Go(CmdZachod, RoomMagicShop),
                      Go(CmdPolnoc, RoomQuestMaster), ]))
                return;
            StreetFights();
        } while (!(Session.Room != RoomShopStreet2));
    }

    public static void Room23() {
        WriteLines([TxtYouAreIn + TxtRoom23_01, TxtRoom23_02, TxtRoom23_03]);
        DescribeStreet();
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdPatrz + CmdPlakat) {
                Out.WriteLine(TxtRoom23_04);
                Out.WriteLine(TxtRoom23_05);
            }
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitZachod, TxtPlaceUlicaSklepikowa, ]);
            if (MoveWhenOrQuit([Go(CmdZachod, RoomShopStreet)])) return;
            BakeryShop();
        } while (!(Session.Room != RoomBakery));
    }

    public static void Room24() {
        WriteLines([TxtYouAreIn + TxtRoom24_01, TxtRoom24_02, TxtRoom24_03]);
        DescribeStreet();
        do {
            ReadCommand();
            ArmouryShop();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceUlicaSklepikowa, ]);
            if (MoveWhenOrQuit([Go(CmdWschod, RoomShopStreet)])) return;
            if (Command == CmdPatrz + CmdPlakat)
                Out.WriteLine(TxtRoom24_04);
            StreetFights();
        } while (!(Session.Room != RoomArmoury));
    }

    public static void Room25() {
        WriteLines([TxtRoom25_01, TxtRoom25_02]);
        DescribeStreet();
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitZachod, TxtPlaceUlicaSklepikowa, ]);
            if (MoveWhenOrQuit([Go(CmdZachod, RoomShopStreet2)])) return;
            if (Command == CmdPatrz + CmdWywieszka)
                Out.WriteLine(TxtRoom25_03);
            StreetFights();
            GeneralStoreShop();
        } while (!(Session.Room != RoomGeneralStore));
    }

    public static void Room26() {
        Out.WriteLine(TxtRoom26_01);
        DescribeStreet();
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceUlicaSklepikowa, ]);
            if (MoveWhenOrQuit([Go(CmdWschod, RoomShopStreet2)])) return;
            if (Command == CmdPatrz + CmdPlakat)
                Out.WriteLine(TxtRoom26_02);
            MagicShop();
            StreetFights();
        } while (!(Session.Room != RoomMagicShop));
    }

    public static void Room30() {
        BasicRoomSimple(RoomLongStreet,
                  [TxtRoom30_01, TxtRoom30_02],
                  [ TxtShared_03,
                   TxtExitWschod, TxtPlaceCentrumMiasta,
                   TxtExitZachod, TxtPlaceUlicaDluga,
                   TxtExitPolnoc, TxtPlaceWejscieNaArene, ],
                  [ Go(CmdWschod, RoomCityCentre),
                   Go(CmdZachod, RoomLongStreet2),
                   Go(CmdPolnoc, RoomArenaEntrance), ]);
    }

    public static void Room31() {
        BasicRoomQuit(RoomLongStreet2,
                  [TxtRoom31_01, TxtRoom31_02],
                  [ TxtShared_03,
                   TxtExitWschod, TxtPlaceUlicaDluga,
                   TxtExitPolnoc, TxtPlaceDolinaRozrywek, ],
                  [Go(CmdWschod, RoomLongStreet), Go(CmdPolnoc, RoomValley)]);
    }

    public static void Room32() {
        BasicRoomQuitPoster(RoomArenaEntrance,
                  [TxtRoom32_01, TxtRoom32_02],
                  [ TxtShared_03,
                   TxtExitPolodnie, TxtPlaceUlicaDluga,
                   TxtExitPolnoc, TxtPlaceArena, ],
                  [Go(CmdPolodnie, RoomLongStreet), Go(CmdPolnoc, RoomArena33)],
                  [TxtRoom32_03, TxtRoom32_04]);
    }

    public static void Room60() {
        BasicRoomSimple(RoomValley,
                  [TxtYouAreIn + TxtRoom60_01, TxtRoom60_02, TxtRoom60_03],
                  [ TxtShared_03,
                   TxtExitPolodnie, TxtPlaceUlicaDluga,
                   TxtExitWschod, TxtPlaceBudowlaPelnaHalasu, ],
                  [Go(CmdPolodnie, RoomLongStreet2), Go(CmdWschod, RoomCrowd1)]);
    }

    public static void Room75() {
        BasicRoomSimple(RoomDarkStreet,
                  TxtRoom75Lines1.Skip(1).ToList(),
                  [ TxtShared_03,
                   TxtExitZachod, TxtPlaceCentrumMiasta,
                   TxtExitPolnoc, TxtPlaceBarPodDwomaPedalami,
                   TxtExitPolodnie, TxtPlaceBluszcz, ],
                  [ Go(CmdZachod, RoomCityCentre), Go(CmdPolnoc, RoomBar),
                   Go(CmdPolodnie, RoomIvy1), ]);
    }

    public static void Room76() {
        Out.WriteLine(TxtYouAreIn + TxtRoom76_01);
        WriteLines(TxtRoom76Lines1.Skip(1).ToList());
        if (World.Monsters[(int)Maciek] == 76)
            Out.WriteLine(TxtRoom76_05);
        if (World.Monsters[(int)Pedal] == 76)
            Out.WriteLine(TxtRoom76_06);
        if (World.Monsters[(int)Couple] == 76)
            Out.WriteLine(TxtRoom76_07);
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlaceUlicaCiemna, ]);
            if (MoveWhenOrQuit([Go(CmdPolodnie, RoomDarkStreet)])) return;
            if (Command == CmdLista)
                WriteLines(TxtRoom76Lines2.Skip(1).ToList());
            if (Command == CmdZabij + CmdPedal) {
                Fight(EasyEnemy);
                World.Monsters[(int)Pedal] = 0;
                PillGain();
            }
            if (Command == CmdZabij + CmdPara) {
                Fight(VeryHardEnemy);
                World.Monsters[(int)Couple] = 0;
                Fight(VeryHardEnemy);
            }
            if (Command == CmdZabij + CmdMaciek) {
                Fight(EasyEnemy);
                World.Monsters[(int)Maciek] = 0;
            }
            if (Command == CmdMam + Cmd18Lat)
                Out.WriteLine(TxtRoom76_13);
        } while (!(Session.Room != RoomBar));
    }

    public static void Room77() {
        WriteLines([TxtRoom77_01, TxtRoom77_02]);
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolnoc, TxtPlaceUlCiemna,
                            TxtExitWschod, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlaceBluszcz,
                            TxtExitPolodnie, TxtPlaceBluszcz, ]);
            if (MoveWhenOrQuit([ Go(CmdPolnoc, RoomDarkStreet),
                      Go(CmdWschod, RoomIvy2),
                      Go(CmdZachod, RoomIvy3),
                      Go(CmdPolodnie, RoomIvy4), ]))
                return;
        } while (!(Session.Room != RoomIvy1));
    }

    public static void Room78() {
        WriteLines([TxtRoom78_01, TxtRoom78_02]);
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitZachod, TxtPlaceBluszcz, ]);
            if (MoveWhenOrQuit([Go(CmdZachod, RoomIvy1)])) return;
        } while (!(Session.Room != RoomIvy2));
    }

    public static void Room79() {
        WriteLines([TxtRoom79_01, TxtRoom79_02]);
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceBluszcz, ]);
            if (MoveWhenOrQuit([Go(CmdWschod, RoomIvy1)])) return;
        } while (!(Session.Room != RoomIvy3));
    }

    public static void Room80() {
        WriteLines(TxtRoom80Lines1.Skip(1).ToList());
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolnoc, TxtPlaceBluszcz,
                            TxtExitWschod, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlaceBluszcz,
                            TxtExitPolodnie, TxtPlaceBluszcz, ]);
            if (MoveWhenOrQuit([ Go(CmdPolnoc, RoomIvy1),
                      Go(CmdZachod, RoomIvy5),
                      Go(CmdWschod, RoomIvy6),
                      Go(CmdPolodnie, RoomClearing), ]))
                return;
            if (Command == CmdPatrz + CmdNapis)
                Out.WriteLine(TxtRoom80_04);
            if (Command == CmdPatrz + CmdStudnia) {
                Combat.Roll = (short)(Roll(3) + 1);
                Out.WriteLine(TxtRoom80_05 + Combat.Roll + TxtShared_23);
                Player.Money = Player.Money + Combat.Roll;
                Combat.Roll = Roll(100);
                if (Combat.Roll < 10) {
                    Out.WriteLine(TxtRoom80_06);
                    Player.Energy = (short)(Player.Energy - 40);
                    Out.WriteLine(TxtRoom80_07);
                    Player.Money = Player.Money - 100;
                    if (Player.Money < 0) Player.Money = 0;
                }
            }
        } while (!(Session.Room != RoomIvy4));
    }

    public static void Room81() {
        WriteLines([TxtRoom81_01, TxtRoom81_02]);
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceBluszcz, ]);
            if (MoveWhenOrQuit([Go(CmdWschod, RoomIvy4)])) return;
        } while (!(Session.Room != RoomIvy5));
    }

    public static void Room82() {
        WriteLines([TxtRoom82_01, TxtRoom82_02]);
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitZachod, TxtPlaceBluszcz, ]);
            if (MoveWhenOrQuit([Go(CmdZachod, RoomIvy4)])) return;
        } while (!(Session.Room != RoomIvy6));
    }

    public static void Room83() {
        WriteLines(TxtRoom83Lines1.Skip(1).ToList());
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolnoc, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlacePodejrzaneKrzaki, ]);
            if (MoveWhenOrExit([Go(CmdPolnoc, RoomIvy4), Go(CmdZachod, RoomCavePath)])) return;
        } while (!(Session.Room != RoomClearing));
    }

    public static void Room84() {
        WriteLines(TxtRoom84Lines1.Skip(1).ToList());
        DescribePlants();
        do {
            ReadCommand();
            ExpandAbbrev();
            PlantFights();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceBluszcz,
                            TxtExitZachod, TxtPlaceBudynek,
                            TxtExitPolodnie, TxtPlaceWejscieDoGroty, ]);
            if (MoveWhenOrQuit([Go(CmdWschod, RoomClearing), Go(CmdZachod, RoomLivingDoor), Go(CmdPolodnie, RoomCaveMouth)])) return;
        } while (!(Session.Room != RoomCavePath));
    }

    public static void Room85() {
        WriteLines([TxtRoom85_01, TxtRoom85_02]);
        if (World.Door != 0)
            Out.WriteLine(TxtRoom85_03);
        do {
            ReadCommand();
            ExpandAbbrev();
            if (Command == CmdZabij + CmdDrzwi) {
                if (World.Door != 0) {
                    Fight(NotEasyEnemy);
                    if (Player.Energy > 0) World.Door = 0;
                }
            }
            CheckMode();
            if (Command == CmdExit) {
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlacePodejrzaneKrzaki, ]);
                if (World.Door != 0)
                    Out.WriteLine(TxtExitZachod + TxtPlaceGdybyNieTeCholerneDrzwi);
                else
                    Out.WriteLine(TxtExitZachod + TxtPlaceWejscieDoDomku);
            }
            if (MoveWhenOrQuit([Go(CmdWschod, RoomCavePath)])) return;
            if (Command == CmdZachod)
                if (World.Door == 0) Session.Room = RoomHut;
                else Out.WriteLine(TxtRoom85_07);
        } while (!(Session.Room != RoomLivingDoor));
    }

    public static void Room86() {
        Out.WriteLine(TxtRoom86_01);
        if (World.OldElf != 0)
            Out.WriteLine(TxtRoom86_02);
        if (World.OldElf == 0)
            Out.WriteLine(TxtRoom86_03);
        do {
            ReadCommand();
            ExpandAbbrev();
            if (Command == CmdDawaj + CmdKunszt) Player.Experience = (short)(Player.Experience + 500);
            if (Command == CmdPatrz + CmdGarnki)
                Out.WriteLine(TxtRoom86_04);
            if (Command == CmdRozmawiaj + CmdStaruch) {
                if ((Bag.Items[(int)TBagItem.Sausage] > -30) && (Player.Money < 200) && (Bag.Items[(int)TBagItem.ReturnScroll] == 0))
                    WriteLines(TxtRoom86Lines1.Skip(1).ToList());
                if ((Bag.Items[(int)TBagItem.Sausage] < -30) && (Player.Money > 199)) {
                    WriteLines(TxtRoom86Lines2.Skip(1).ToList());
                    Session.Room = RoomCityCentre;
                    Bag.Items[(int)TBagItem.Beer] = (short)(Bag.Items[(int)TBagItem.Beer] - ItemUnit);
                    Bag.Items[(int)TBagItem.Sausage] = (short)(Bag.Items[(int)TBagItem.Sausage] + 40);
                    Player.Money = Player.Money - 200;
                    Bag.Items[(int)TBagItem.ReturnScroll] = (short)(Bag.Items[(int)TBagItem.ReturnScroll] - ItemUnit);
                    Player.Experience = (short)(Player.Experience + 50);
                    Bag.Count = (short)(Bag.Count - 2);
                }
                if (Bag.Items[(int)TBagItem.ReturnScroll] < 0) {
                    Out.WriteLine(TxtRoom86_13);
                    Session.Room = RoomCageHuge;
                }
            }
            if (Command == CmdZabij + CmdStaruch) {
                if (World.OldElf != 0) {
                    Fight(WeakEnemy);
                    World.OldElf = 0;
                    Out.WriteLine(TxtRoom86_14);
                    Out.WriteLine(TxtRoom86_15);
                    Player.Level = (sbyte)(Player.Level - 1);
                }
            }
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitWschod, TxtPlaceBudynek, ]);
            if (MoveWhenOrQuit([Go(CmdWschod, RoomLivingDoor)])) return;
        } while (!(Session.Room != RoomHut));
    }

    public static void Room87() {
        Out.WriteLine(TxtRoom87_01);
        Out.WriteLine(TxtRoom87_02);
        Combat.Roll = Roll(100);
        if (Combat.Roll < 40) {
            Out.WriteLine(TxtRoom87_03 + (25 - Player.Stats[(int)TStat.Dexterity]) + TxtShared_26);
            Player.Energy = (short)(Player.Energy - 25 + Player.Stats[(int)TStat.Dexterity]);
        }
        do {
            ReadCommand();
            ExpandAbbrev();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlaceGrota,
                            TxtExitPolnoc, TxtPlacePodejrzaneKrzaki, ]);
            if (MoveWhenOrQuit([Go(CmdPolnoc, RoomCavePath), Go(CmdPolodnie, RoomNettleThrone)])) return;
        } while (!(Session.Room != RoomCaveMouth));
    }

    public static void Room88() {
        WriteLines(TxtRoom88Lines1.Skip(1).ToList());
        Out.Write(TxtRoom88_05);
        Command = Crt.ReadLn();
        Out.WriteLine(TxtRoom88_06);
        do {
            Combat.EnemyEnergy = 200;
            Combat.EnemyDexterity = 15;
            Combat.EnemyStrength = 18;
            Combat.FireballCharges = 5;
            Combat.PoisonCharges = 1;
            DoFight();
            if (Combat.EnemyEnergy < 0) {
                LeafGain();
                PillGain();
                ScrollGain();
                if (World.DuncanQuest != 0) World.DuncanQuest = (sbyte)(World.DuncanQuest - 50);
            }
            if (Player.Energy > 0) {
                Out.WriteLine(TxtRoom88_07);
                Session.Room = RoomCaveMouth;
            }
            ReadCommand();
            ExpandAbbrev();
            CheckMode();
            if (Command == CmdExit)
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlaceWejscieDoGroty2, ]);
            if (MoveWhenOrQuit([Go(CmdPolodnie, RoomCaveMouth)])) return;
        } while (!(Session.Room != RoomNettleThrone));
    }

    public static void GrantQuestReward(short quest, short exp) {
        Out.WriteLine(TxtRoom100_12);
        Out.WriteLine(TxtQuestReward + exp + TxtSpaceKunsztDashes);
        Player.Experience = (short)(Player.Experience + exp);
        World.Quest = 0;
        World.QuestKills = 0;
        Bag.Permit = Bag.Permit - ItemUnit;
        switch (quest) {
            case 1:
                Bag.Count = (short)(Bag.Count + 1);
                break;
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

    public static void Room100() {
        WriteLines([TxtRoom100_01, TxtRoom100_02]);
        DescribeStreet();
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit) {
                WriteExits([ TxtShared_03,
                            TxtExitPolodnie, TxtPlaceUlSklepikowa,
                            TxtExitWschod, TxtPlaceDrogaOKtorejMozeszPomazycBoJestWProdukcji, ]);
                if (Bag.Permit == 0) Out.WriteLine(TxtExitZachod + TxtPlaceDrogaZablokowanaPrzezQuestMastera);
                if (Bag.Permit < 0) Out.WriteLine(TxtExitZachod + TxtPlaceDroga);
            }
            if (Command == CmdLista)
                WriteLines(TxtRoom100Lines1.Skip(1).ToList());
            for (int i = 1; i < QuestOffers.Count; i++) {
                TQuestOffer q = QuestOffers[i];
                BuyQuest(q.cmd, q.line, q.quest, q.kills, q.price);
            }
            if ((Command == CmdSprzedaj + CmdQuest) && (World.Quest > 0)) {
                switch (World.Quest) {
                    case 1:
                        if (World.QuestKills < 1) GrantQuestReward(1, 100);
                        break;
                    case 2:
                        if ((World.QuestKills < 1) && (Bag.Items[(int)TBagItem.Diploma] <= -ItemUnit)) GrantQuestReward(2, 250);
                        break;
                    case 3:
                        if ((World.QuestKills < 1) && (Bag.Items[(int)TBagItem.Pipe] <= -ItemUnit) && (Player.Practice > 0)) GrantQuestReward(3, 425);
                        break;
                }
            }
            if (MoveWhenOrExit([Go(CmdPolodnie, RoomShopStreet2)])) return;
            if ((Command == CmdZachod) && (Bag.Permit <= -ItemUnit)) Session.Room = RoomRoad1;
            if ((Command == CmdZachod) && (Bag.Permit == 0)) Out.WriteLine(TxtRoom100_16);
            if (Command == CmdZabij + CmdQuestMaster) {
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

    public static void Room101() {
        BasicRoomQuitGeneric(RoomRoad1,
                  TxtRoom101Lines1.Skip(1).ToList(),
                  [ TxtShared_03,
                   TxtExitWschod, TxtPlaceRozjazd,
                   TxtExitZachod, TxtPlaceDroga,
                   TxtExitPolnoc, TxtPlaceLas,
                   TxtExitPolodnie, TxtPlaceLas, ],
                  [ Go(CmdWschod, RoomQuestMaster), Go(CmdZachod, RoomRoad2),
                   Go(CmdPolnoc, RoomForest), ],
                  CmdPatrz + CmdDrogowskaz,
                  [TxtRoom101_05]);
    }

    public static void Room102() {
        BasicRoomQuit(RoomRoad2,
                  [TxtRoom102_01, TxtRoom102_02],
                  [ TxtShared_03,
                   TxtExitWschod, TxtPlaceDroga,
                   TxtExitZachod, TxtPlaceDroga,
                   TxtExitPolnoc, TxtPlaceLas,
                   TxtExitPolodnie, TxtPlaceLas, ],
                  [Go(CmdWschod, RoomRoad1), Go(CmdZachod, RoomRoad3)]);
    }

    public static void Room103() {
        BasicRoomQuit(RoomRoad3,
                  TxtRoom103Lines1.Skip(1).ToList(),
                  [ TxtShared_03,
                   TxtExitWschod, TxtPlaceDroga,
                   TxtExitPolnoc, TxtPlaceLas,
                   TxtExitPolodnie, TxtPlaceLas, ],
                  [Go(CmdWschod, RoomRoad2)]);
    }

    public static void ConcertRooms() {
        if (Session.Room == RoomCrowd1) {
            WriteLines(TxtConcertRoomsLines1.Skip(1).ToList());
            DescribeCrowd();
            do {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitZachod, TxtPlaceDolinaRozrywek,
                                TxtExitPolnoc, TxtPlaceTlum,
                                TxtExitPolodnie, TxtPlaceTlum, ]);
                ConcertFights();
                if (MoveWhenOrExit([ Go(CmdZachod, RoomValley),
                          Go(CmdPolnoc, RoomCrowd3),
                          Go(CmdPolodnie, RoomCrowd2),
                          Go(CmdWschod, RoomCrowd4), ]))
                    return;
                if (Command == CmdPatrz + CmdPlakat) {
                    Out.WriteLine(TxtConcertRooms_07);
                    Out.WriteLine(TxtConcertRooms_08);
                }
            } while (!(Session.Room != RoomCrowd1));
        }
        if (Session.Room == RoomCrowd2) {
            Out.WriteLine(TxtConcertRooms_09);
            DescribeCrowd();
            do {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitPolnoc, TxtPlaceTlum,
                                TxtExitWschod, TxtPlaceTlum, ]);
                ConcertFights();
                if (MoveWhenOrExit([Go(CmdPolnoc, RoomCrowd1), Go(CmdWschod, RoomCrowd5)])) return;
            } while (!(Session.Room != RoomCrowd2));
        }
        if (Session.Room == RoomCrowd3) {
            Out.WriteLine(TxtConcertRooms_11);
            DescribeCrowd();
            do {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitPolodnie, TxtPlaceTlum,
                                TxtExitWschod, TxtPlaceTlum,
                                TxtExitPolnoc, TxtPlacePiwiarnia, ]);
                ConcertFights();
                if (MoveWhenOrExit([Go(CmdPolodnie, RoomCrowd1), Go(CmdWschod, RoomCrowd6), Go(CmdPolnoc, RoomPub)])) return;
            } while (!(Session.Room != RoomCrowd3));
        }
        if (Session.Room == RoomCrowd4) {
            Out.WriteLine(TxtYouAreIn + TxtConcertRooms_13);
            Out.WriteLine(TxtConcertRooms_14);
            DescribeCrowd();
            do {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitZachod, TxtPlaceTlum,
                                TxtExitPolnoc, TxtPlaceTlum,
                                TxtExitPolodnie, TxtPlaceTlum, ]);
                ConcertFights();
                if (MoveWhenOrExit([Go(CmdPolnoc, RoomCrowd6), Go(CmdZachod, RoomCrowd1), Go(CmdPolodnie, RoomCrowd5)])) return;
            } while (!(Session.Room != RoomCrowd4));
        }
        if (Session.Room == RoomCrowd5) {
            Out.WriteLine(TxtConcertRooms_16);
            Out.WriteLine(TxtConcertRooms_14);
            DescribeCrowd();
            do {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitZachod, TxtPlaceTlum,
                                TxtExitPolnoc, TxtPlaceTlum, ]);
                ConcertFights();
                if (MoveWhenOrExit([Go(CmdPolnoc, RoomCrowd4), Go(CmdZachod, RoomCrowd2)])) return;
            } while (!(Session.Room != RoomCrowd5));
        }
        if (Session.Room == RoomCrowd6) {
            Out.WriteLine(TxtConcertRooms_17);
            DescribeCrowd();
            do {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitZachod, TxtPlaceTlum,
                                TxtExitPolodnie, TxtPlaceTlum, ]);
                ConcertFights();
                if (MoveWhenOrExit([Go(CmdPolodnie, RoomCrowd4), Go(CmdZachod, RoomCrowd3)])) return;
            } while (!(Session.Room != RoomCrowd6));
        }
        if (Session.Room == RoomPub) {
            Out.WriteLine(TxtYouAreIn + TxtConcertRooms_18);
            Out.WriteLine(TxtConcertRooms_19);
            if (World.Monsters[(int)Minibarman] == 67)
                Out.WriteLine(TxtConcertRooms_20);
            if (World.Monsters[(int)Fatso] == 67)
                Out.WriteLine(TxtConcertRooms_21);
            DescribeCrowd();
            do {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitWschod, TxtPlaceWejscieNaEstrade,
                                TxtExitPolodnie, TxtPlaceTlum, ]);
                ConcertFights();
                if ((Command == CmdZabij + CmdMiniBarman) && (World.Monsters[(int)Minibarman] == 67)) {
                    Out.WriteLine(TxtConcertRooms_23);
                    Out.WriteLine(TxtConcertRooms_24);
                    Fight(VeryHardEnemy);
                    if (Player.Energy > 0) {
                        World.Monsters[(int)Minibarman] = 0;
                        Combat.Roll = Roll(100);
                        if (Combat.Roll < 15) {
                            Out.WriteLine(TxtConcertRooms_25 + TxtShared_08 + TxtPiwoTail);
                            Bag.Items[(int)TBagItem.Beer] = (short)(Bag.Items[(int)TBagItem.Beer] - ItemUnit);
                            Bag.Count = (short)(Bag.Count + 1);
                        }
                    }
                }
                if ((Command == CmdZabij + CmdGrubas) && (World.Monsters[(int)Fatso] == 67)) {
                    Fight(VeryHardEnemy);
                    if (Player.Energy > 0) {
                        World.Monsters[(int)Fatso] = 0;
                        if (Combat.Roll < 15) {
                            Out.WriteLine(TxtConcertRooms_26 + TxtShared_08 + TxtPiwoTail);
                            Bag.Items[(int)TBagItem.Beer] = (short)(Bag.Items[(int)TBagItem.Beer] - ItemUnit);
                            Bag.Count = (short)(Bag.Count + 1);
                        }
                    }
                }
                if (MoveWhenOrExit([Go(CmdPolodnie, RoomCrowd3), Go(CmdWschod, RoomStageEntrance)])) return;
            } while (!(Session.Room != RoomPub));
        }
        if (Session.Room == RoomStageEntrance) {
            Out.WriteLine(TxtConcertRooms_27);
            Out.WriteLine(TxtConcertRooms_28);
            DescribeCrowd();
            do {
                ReadCommand();
                ExpandAbbrev();
                CheckMode();
                if (Command == CmdExit) {
                    WriteExits([ TxtShared_03,
                                TxtExitZachod, TxtPlacePiwiarnia,
                                TxtExitWschod, TxtPlaceScena, ]);
                    Out.WriteLine(TxtConcertRooms_31);
                }
                ConcertFights();
                if (MoveWhenOrExit([Go(CmdZachod, RoomPub), Go(CmdWschod, RoomBackstage)])) return;
                if (Command == CmdPatrz + CmdTabliczka)
                    Out.WriteLine(TxtConcertRooms_32);
            } while (!(Session.Room != RoomStageEntrance));
        }
        if (Session.Room == RoomBackstage) {
            Out.WriteLine(TxtConcertRooms_33);
            Out.WriteLine(TxtConcertRooms_34);
            if (World.Monsters[(int)DJ] == 69)
                Out.WriteLine(TxtConcertRooms_35);
            do {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitZachod, TxtPlaceWejscieNaScene,
                                TxtExitPolodnie, TxtPlaceScena, ]);
                if (Command == CmdZabij + CmdDJ) {
                    if (World.Monsters[(int)DJ] == 69) {
                        Fight(VeryHardEnemy);
                        if (Combat.EnemyEnergy < 0) {
                            World.Monsters[(int)DJ] = 0;
                            Combat.Roll = Roll(100);
                            if (Combat.Roll < 25) {
                                Out.WriteLine(TxtShared_08 + TxtConcertRooms_38);
                                Bag.Items[(int)TBagItem.DryRation] = (short)(Bag.Items[(int)TBagItem.DryRation] - ItemUnit);
                                Bag.Count = (short)(Bag.Count + 1);
                            }
                            if (Combat.Roll < 3) {
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
                if (MoveWhenOrExit([Go(CmdZachod, RoomStageEntrance), Go(CmdPolodnie, RoomStage1)])) return;
            } while (!(Session.Room != RoomBackstage));
        }
        if (Session.Room == RoomStage1) {
            WriteLines(TxtConcertRoomsLines2.Skip(1).ToList());
            DescribeStage();
            do {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitPolnoc, TxtPlaceScena,
                                TxtExitPolodnie, TxtPlaceSrodekSceny, ]);
                if (MoveWhenOrExit([Go(CmdPolnoc, RoomBackstage), Go(CmdPolodnie, RoomStage2)])) return;
                StageFights();
            } while (!(Session.Room != RoomStage1));
        }
        if (Session.Room == RoomStage2) {
            Out.WriteLine(TxtConcertRooms_46);
            Out.WriteLine(TxtConcertRooms_47);
            DescribeStage();
            do {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitPolnoc, TxtPlaceScena,
                                TxtExitPolodnie, TxtPlaceScena, ]);
                if (MoveWhenOrExit([Go(CmdPolnoc, RoomStage1), Go(CmdPolodnie, RoomStage3)])) return;
                StageFights();
            } while (!(Session.Room != RoomStage2));
        }
        if (Session.Room == RoomStage3) {
            Out.WriteLine(TxtConcertRooms_48);
            Out.WriteLine(TxtConcertRooms_49);
            DescribeStage();
            do {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit)
                    WriteExits([ TxtShared_03,
                                TxtExitPolnoc, TxtPlaceSrodekSceny, ]);
                if (MoveWhenOrExit([Go(CmdPolnoc, RoomStage2)])) return;
                StageFights();
            } while (!(Session.Room != RoomStage3));
        }
    }

    public static void PlainRooms() {
        for (int i = 1; i < SideRooms.Count; i++) {
            TSideRoom s = SideRooms[i];
            SimpleRoom(s.room, s.dest, s.direction, s.exitDesc);
        }
    }

    public static void BasicRoomGeneric(short RoomNo, List<string> intro, List<string> exits,
        List<TMovement> moves, string posterCmd, List<string> poster) {
        if (Session.Room != RoomNo) return;
        WriteLines(intro);
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(exits);
            if (MoveWhenOrExit(moves)) return;
            if (poster.Any() && (Command == posterCmd))
                WriteLines(poster);
        } while (!(Session.Room != RoomNo));
    }

    public static void BasicRoomPoster(short RoomNo, List<string> intro, List<string> exits,
        List<TMovement> moves, List<string> poster) {
        BasicRoomGeneric(RoomNo, intro, exits, moves, CmdPatrz + CmdPlakat, poster);
    }

    public static void BasicRoomQuit(short RoomNo, List<string> intro, List<string> exits,
        List<TMovement> moves) {
        BasicRoomQuitGeneric(RoomNo, intro, exits, moves, "", []);
    }

    public static void BasicRoomQuitGeneric(short RoomNo, List<string> intro, List<string> exits,
        List<TMovement> moves, string posterCmd, List<string> poster) {
        if (Session.Room != RoomNo) return;
        WriteLines(intro);
        do {
            ReadCommand();
            CheckMode();
            if (Command == CmdExit)
                WriteExits(exits);
            if (MoveWhenOrQuit(moves)) return;
            if (poster.Any() && (Command == posterCmd))
                WriteLines(poster);
        } while (!(Session.Room != RoomNo));
    }

    public static void BasicRoomQuitPoster(short RoomNo, List<string> intro, List<string> exits,
        List<TMovement> moves, List<string> poster) {
        BasicRoomQuitGeneric(RoomNo, intro, exits, moves, CmdPatrz + CmdPlakat, poster);
    }

    public static void BasicRoomSimple(short RoomNo, List<string> intro, List<string> exits,
        List<TMovement> moves) {
        BasicRoomGeneric(RoomNo, intro, exits, moves, "", []);
    }

    public static void SimpleRoom(short RoomNo, short DestRoom, string Direction, string ExitDesc) {
        if (Session.Room == RoomNo) {
            Out.WriteLine(TxtYouAreIn + TxtSimpleRoom_01);
            do {
                ReadCommand();
                CheckMode();
                if (Command == CmdExit) {
                    Out.WriteLine(TxtShared_03);
                    Out.WriteLine(Direction + TxtSimpleRoom_02 + ExitDesc);
                }
                if (MoveWhenOrExit([Go(Direction, DestRoom)])) return;
            } while (!(Session.Room != RoomNo));
        }
    }

    public class TRoomEntry {
        public short id;
        public Action run;
        public TRoomEntry(short id, Action run) {
            this.id = id;
            this.run = run;
        }
    }

    public static readonly List<TRoomEntry> RoomDispatch = [ null!,
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
    ];

    public static void DispatchRoom() {
        for (int ri = 1; ri < RoomDispatch.Count; ri++)
            if ((RoomDispatch[ri].id == -1) || (Session.Room == RoomDispatch[ri].id)) {
                RoomDispatch[ri].run();
                if (Session.ExitRequested) return;
            }
    }
}
