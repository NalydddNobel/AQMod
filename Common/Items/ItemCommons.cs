namespace Aequus.Common.Items;

public static class ItemCommons {
    public class Rarity {
        public const int SkyMerchantShopItem = ItemRarityID.Blue;
        public const int PollutedOceanLoot = ItemRarityID.Blue;
        public const int AquaticBossLoot = ItemRarityID.Blue;
        public const int JungleLoot = ItemRarityID.Green;
        public const int GlimmerLoot = ItemRarityID.Green;
        public const int DungeonLoot = ItemRarityID.Green;
        public const int DemonSiegeTier1Loot = ItemRarityID.Orange;
        public const int OmegaStariteLoot = ItemRarityID.LightRed;
        public const int WallOfFleshLoot = ItemRarityID.LightRed;
        public const int EarlyHardmodeMaterial = ItemRarityID.Orange;
        public const int HardSandstormBlizzardLoot = ItemRarityID.LightRed;
        public const int SpaceStormLoot = ItemRarityID.Pink;
        public const int DustDevilLoot = ItemRarityID.Pink;
        public const int PlanteraLoot = ItemRarityID.Yellow;
        public const int HardDungeonLoot = ItemRarityID.Yellow;
        public const int DemonSiegeTier2Loot = ItemRarityID.Yellow;
        public const int UpriserLoot = ItemRarityID.Cyan;
        public const int MoonLordLoot = ItemRarityID.Red;
        public const int YinYangLoot = ItemRarityID.Red;

        public const int OccultistCrown = ItemRarityID.LightRed;
        public const int ShimmerPermaPowerup = ItemRarityID.LightPurple;
    }

    public class Price {
        /// <summary>Sell price: 1 Gold</summary>
        public static int SkyMerchantShopItem { get; set; } = Item.sellPrice(gold: 1);

        /// <summary>Buy price: 5 Gold</summary>
        public static int SkyMerchantCustomPurchasePrice { get; set; } = Item.buyPrice(gold: 5);

        /// <summary>Sell price: 50 Silver</summary>
        public static int PollutedOceanLoot { get; set; } = Item.sellPrice(silver: 50);

        /// <summary>Sell price: 75 Silver</summary>
        public static int AquaticBossLoot { get; set; } = Item.sellPrice(silver: 75);

        /// <summary>Sell price: 1 Gold</summary>
        public static int GlimmerLoot { get; set; } = Item.sellPrice(gold: 1);

        /// <summary>Sell price: 1 Gold 75 Silver</summary>
        public static int DungeonLoot { get; set; } = Item.sellPrice(gold: 1, silver: 75);

        /// <summary>Sell price: 2 Gold</summary>
        public static int DemonSiegeLoot { get; set; } = Item.sellPrice(gold: 2);

        /// <summary>Sell price: 3 Gold</summary>
        public static int OmegaStariteLoot { get; set; } = Item.sellPrice(gold: 3);

        /// <summary>Sell price: 2 Gold</summary>
        public static int HardSandstormBlizzardLoot { get; set; } = Item.sellPrice(gold: 2);

        /// <summary>Sell price: 3 Gold</summary>
        public static int SpaceStormLoot { get; set; } = Item.sellPrice(gold: 3);

        /// <summary>Sell price: 3 Gold 50 Silver</summary>
        public static int DustDevilLoot { get; set; } = Item.sellPrice(gold: 3, silver: 50);

        /// <summary>Sell price: 6 Gold</summary>
        public static int HardDungeonLoot { get; set; } = Item.sellPrice(gold: 6);

        /// <summary>Sell price: 8 Gold</summary>
        public static int DemonSiegeTier2Loot { get; set; } = Item.sellPrice(gold: 8);

        /// <summary>Sell price: 8 Gold 50 Silver</summary>
        public static int UpriserLoot { get; set; } = Item.sellPrice(gold: 8, silver: 50);

        /// <summary>Sell price: 10 Gold</summary>
        public static int YinYangLoot { get; set; } = Item.sellPrice(gold: 10);

        /// <summary>Buy price: 30 Gold</summary>
        public static int OccultistCrown { get; set; } = Item.buyPrice(gold: 30);
    }
}