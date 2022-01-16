using Terraria;

namespace AQMod.Common.ID
{
    internal static class ChestStyles
    {
        public const int Wood = 0;
        public const int Gold = 1;
        public const int LockedGold = 2;
        public const int Shadow = 3;
        public const int LockedShadow = 4;
        public const int Barrel = 5;
        public const int TrashCan = 6;
        public const int Ebonwood = 7;
        public const int RichMahogany = 8;
        public const int Pearlwood = 9;
        public const int Ivy = 10;
        public const int Ice = 11;
        public const int Living = 12;
        public const int Skyware = 13;
        public const int Shadewood = 14;
        public const int Webbed = 15;
        public const int Lihzahrd = 16;
        public const int Ocean = 17;
        public const int Jungle = 18;
        public const int Corruption = 19;
        public const int Crimson = 20;
        public const int Hallow = 21;
        public const int Frozen = 22;
        public const int LockedJungle = 23;
        public const int LockedCorruption = 24;
        public const int LockedCrimson = 25;
        public const int LockedHallow = 26;
        public const int LockedFrozen = 27;
        public const int Dynasty = 28;
        public const int Honey = 29;
        public const int Steampunk = 30;
        public const int Palm = 31;
        public const int Mushroom = 32;
        public const int Boreal = 33;
        public const int Slime = 34;
        public const int GreenDungeon = 35;
        public const int LockedGreenDungeon = 36;
        public const int PinkDungeon = 37;
        public const int LockedPinkDungeon = 38;
        public const int BlueDungeon = 39;
        public const int LockedBlueDungeon = 40;
        public const int Bone = 41;
        public const int Cactus = 42;
        public const int Flesh = 43;
        public const int Obsidian = 44;
        public const int Pumpkin = 45;
        public const int Spooky = 46;
        public const int Glass = 47;
        public const int Martian = 48;
        public const int Meteor = 49;
        public const int Granite = 50;
        public const int Marble = 51;
        public const int _jeweled = 52;
        public const int _golden = 53;

        public static int GetChestStyle(Chest chest)
        {
            return GetChestStyle(Main.tile[chest.x, chest.y]);
        }

        public static int GetChestStyle(Tile tile)
        {
            return GetChestStyle(tile.frameX);
        }

        public static int GetChestStyle(int frameX)
        {
            return frameX / 36;
        }
    }
}
