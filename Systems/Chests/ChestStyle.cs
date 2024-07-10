namespace AequusRemake.Systems.Chests;

public enum ChestStyle : ushort {
    /* Containers */
    Wood = ChestStyleConversion.CONTAINERS_START,
    Gold,
    LockedGold,
    Shadow,
    LockedShadow,
    Barrel,
    TrashCan,
    Ebonwood,
    RichMahogany,
    Pearlwood,
    Ivy,
    Frozen,
    Living,
    Skyware,
    Shadewood,
    Webbed,
    Lihzahrd,
    Ocean,
    Jungle,
    Corruption,
    Crimson,
    Hallow,
    Ice,
    LockedJungle,
    LockedCorruption,
    LockedCrimson,
    LockedHallow,
    LockedIce,
    Dynasty,
    Honey,
    Steampunk,
    Palm,
    Mushroom,
    Boreal,
    Slime,
    GreenDungeon,
    LockedGreenDungeon,
    PinkDungeon,
    LockedPinkDungeon,
    BlueDungeon,
    LockedBlueDungeon,
    Bone,
    Cactus,
    Flesh,
    Obsidian,
    Pumpkin,
    Spooky,
    Glass,
    Martian,
    Meteor,
    Granite,
    Marble,

    /* Containers2 */
    Jeweled = ChestStyleConversion.CONTAINERS2_START,
    PirateGolden,
    Spider,
    Lesion,
    DeadMans,
    Solar,
    Vortex,
    Nebula,
    Stardust,
    Golf,
    Sandstone,
    Bamboo,
    Desert,
    LockedDesert,
    Reef,
    Balloon,
    Ashwood,

    /* AequusRemake */
    Oblivion = ChestStyleConversion.AequusRemake_START,
    Dumpster,

    Unknown = ushort.MaxValue
}

public class ChestStyleConversion {
    internal const ushort CONTAINERS_START = 0;
    internal const ushort CONTAINERS2_START = 100;
    internal const ushort AequusRemake_START = 200;

    public static void ToTile(ChestStyle style, out int chestType, out int chestStyle) {
        chestType = TileID.Containers;
        chestStyle = (int)style;

        if (chestStyle < CONTAINERS2_START) {
            return;
        }

        if (chestStyle < AequusRemake_START) {
            chestType = TileID.Containers2;
            chestStyle -= CONTAINERS2_START;
            return;
        }

        // TODO -- Add impl for enum -> AequusRemake chest
    }
    public static ChestStyle ToEnum(int chestType, int chestStyle) {
        if (chestType == TileID.Containers) {
            return (ChestStyle)chestStyle;
        }

        if (chestType == TileID.Containers2) {
            return (ChestStyle)(chestStyle + CONTAINERS2_START);
        }

        // TODO -- Add impl for AequusRemake chest -> enum
        return ChestStyle.Unknown;
    }

    public static ChestStyle ToEnum(Tile chestTile) {
        return ToEnum(chestTile.TileType, chestTile.TileFrameX / 36);
    }
}