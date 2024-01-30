using System.Collections.Generic;

namespace Aequus.Common.Tiles;

public class ChestType : ModSystem {
    public static HashSet<TileKey> GenericUndergroundChest { get; private set; }

    public const System.Int32 Wood = 0;
    public const System.Int32 Gold = 1;
    public const System.Int32 LockedGold = 2;
    public const System.Int32 Shadow = 3;
    public const System.Int32 LockedShadow = 4;
    public const System.Int32 Barrel = 5;
    public const System.Int32 TrashCan = 6;
    public const System.Int32 Ebonwood = 7;
    public const System.Int32 RichMahogany = 8;
    public const System.Int32 Pearlwood = 9;
    public const System.Int32 Ivy = 10;
    public const System.Int32 Frozen = 11;
    public const System.Int32 Living = 12;
    public const System.Int32 Skyware = 13;
    public const System.Int32 Shadewood = 14;
    public const System.Int32 Webbed = 15;
    public const System.Int32 Lihzahrd = 16;
    public const System.Int32 Ocean = 17;
    public const System.Int32 Jungle = 18;
    public const System.Int32 Corruption = 19;
    public const System.Int32 Crimson = 20;
    public const System.Int32 Hallow = 21;
    public const System.Int32 Ice = 22;
    public const System.Int32 LockedJungle = 23;
    public const System.Int32 LockedCorruption = 24;
    public const System.Int32 LockedCrimson = 25;
    public const System.Int32 LockedHallow = 26;
    public const System.Int32 LockedIce = 27;
    public const System.Int32 Dynasty = 28;
    public const System.Int32 Honey = 29;
    public const System.Int32 Steampunk = 30;
    public const System.Int32 Palm = 31;
    public const System.Int32 Mushroom = 32;
    public const System.Int32 Boreal = 33;
    public const System.Int32 Slime = 34;
    public const System.Int32 GreenDungeon = 35;
    public const System.Int32 LockedGreenDungeon = 36;
    public const System.Int32 PinkDungeon = 37;
    public const System.Int32 LockedPinkDungeon = 38;
    public const System.Int32 BlueDungeon = 39;
    public const System.Int32 LockedBlueDungeon = 40;
    public const System.Int32 Bone = 41;
    public const System.Int32 Cactus = 42;
    public const System.Int32 Flesh = 43;
    public const System.Int32 Obsidian = 44;
    public const System.Int32 Pumpkin = 45;
    public const System.Int32 Spooky = 46;
    public const System.Int32 Glass = 47;
    public const System.Int32 Martian = 48;
    public const System.Int32 Meteor = 49;
    public const System.Int32 Granite = 50;
    public const System.Int32 Marble = 51;
    public const System.Int32 ContainersCount = 52;

    public const System.Int32 Jeweled = 0;
    public const System.Int32 PirateGolden = 1;
    public const System.Int32 Spider = 2;
    public const System.Int32 Lesion = 3;
    public const System.Int32 DeadMans = 4;
    public const System.Int32 Solar = 5;
    public const System.Int32 Vortex = 6;
    public const System.Int32 Nebula = 7;
    public const System.Int32 Stardust = 8;
    public const System.Int32 Golf = 9;
    public const System.Int32 Sandstone = 10;
    public const System.Int32 Bamboo = 11;
    public const System.Int32 Desert = 12;
    public const System.Int32 LockedDesert = 13;
    public const System.Int32 Reef = 14;
    public const System.Int32 Balloon = 15;
    public const System.Int32 Ashwood = 16;
    public const System.Int32 Containers2Count = 14;

    public override void Load() {
        GenericUndergroundChest = new HashSet<TileKey>() {
            new(TileID.Containers, Gold),
            new(TileID.Containers, Frozen),
            new(TileID.Containers, Marble),
            new(TileID.Containers, Granite),
            new(TileID.Containers, Mushroom),
            new(TileID.Containers, Ivy),
            new(TileID.Containers, Webbed),
            new(TileID.Containers, RichMahogany),
            new(TileID.Containers, Webbed),
            new(TileID.Containers, Ocean),
            new(TileID.Containers2, Sandstone),
            new(TileID.Containers2, Spider),
        };
    }

    public override void Unload() {
        GenericUndergroundChest?.Clear();
        GenericUndergroundChest = null;
    }
}