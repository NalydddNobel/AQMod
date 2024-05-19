namespace Aequus.Common;

public partial class Commons {
    public class Rare {
        public const int SkyMerchantShopItem = 1;
        public const int PollutedOceanLoot = 1;
        public const int AquaticBossLoot = 1;
        public const int JungleLoot = 2;
        public const int GlimmerLoot = 2;
        public const int DungeonLoot = 2;
        public const int DemonSiegeLoot = 3;
        public const int OmegaStariteLoot = 4;
        public const int WallOfFleshLoot = 4;
        public const int EarlyHardmodeMaterial = 3;
        public const int HardSandstormBlizzardLoot = 4;
        public const int SpaceStormLoot = 5;
        public const int DustDevilLoot = 5;
        public const int PlanteraLoot = 8;
        public const int HardDungeonLoot = 8;
        public const int DemonSiegeTier2Loot = 8;
        public const int UpriserLoot = 9;
        public const int MoonLordLoot = 10;
        public const int YinYangLoot = 10;

        public const int ShimmerPermaPowerup = 6;
    }

    public class Cost {
        /// <summary>1 Gold</summary>
        public static int SkyMerchantShopItem { get; set; } = Item.sellPrice(gold: 1);

        /// <summary>5 Gold</summary>
        public static int SkyMerchantCustomPurchasePrice { get; set; } = Item.buyPrice(gold: 5);

        /// <summary>50 Silver</summary>
        public static int PollutedOceanLoot { get; set; } = Item.sellPrice(silver: 50);

        /// <summary>75 Silver</summary>
        public static int AquaticBossLoot { get; set; } = Item.sellPrice(silver: 75);

        /// <summary>1 Gold</summary>
        public static int GlimmerLoot { get; set; } = Item.sellPrice(gold: 1);

        /// <summary>1 Gold 75 Silver</summary>
        public static int DungeonLoot { get; set; } = Item.sellPrice(gold: 1, silver: 75);

        /// <summary>2 Gold</summary>
        public static int DemonSiegeLoot { get; set; } = Item.sellPrice(gold: 2);

        /// <summary>3 Gold</summary>
        public static int OmegaStariteLoot { get; set; } = Item.sellPrice(gold: 3);

        /// <summary>2 Gold</summary>
        public static int HardSandstormBlizzardLoot { get; set; } = Item.sellPrice(gold: 2);

        /// <summary>3 Gold</summary>
        public static int SpaceStormLoot { get; set; } = Item.sellPrice(gold: 3);

        /// <summary>3 Gold 50 Silver</summary>
        public static int DustDevilLoot { get; set; } = Item.sellPrice(gold: 3, silver: 50);

        /// <summary>6 Gold</summary>
        public static int HardDungeonLoot { get; set; } = Item.sellPrice(gold: 6);

        /// <summary>8 Gold</summary>
        public static int DemonSiegeTier2Loot { get; set; } = Item.sellPrice(gold: 8);

        /// <summary>8 Gold 50 Silver</summary>
        public static int UpriserLoot { get; set; } = Item.sellPrice(gold: 8, silver: 50);

        /// <summary>10 Gold</summary>
        public static int YinYangLoot { get; set; } = Item.sellPrice(gold: 10);
    }
}