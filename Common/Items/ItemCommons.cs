namespace Aequus.Common.Items;

public static class ItemCommons {
    public class Rarity {
        public static System.Int32 SkyMerchantShopItem = ItemRarityID.Blue;
        public static System.Int32 PollutedOceanLoot = ItemRarityID.Blue;
        public static System.Int32 CrabsonLoot = ItemRarityID.Blue;
        public static System.Int32 GlimmerLoot = ItemRarityID.Green;
        public static System.Int32 DemonSiegeTier1Loot = ItemRarityID.Orange;
        public static System.Int32 OmegaStariteLoot = ItemRarityID.LightRed;
        public static System.Int32 SpaceStormLoot = ItemRarityID.Pink;
        public static System.Int32 DustDevilLoot = ItemRarityID.LightPurple;
        public static System.Int32 DemonSiegeTier2Loot = ItemRarityID.Yellow;
        public static System.Int32 UpriserLoot = ItemRarityID.Red;
        public static System.Int32 YinYangLoot = ItemRarityID.Red;

        #region Vanilla
        public const System.Int32 ShimmerPermaPowerup = ItemRarityID.LightPurple;
        public const System.Int32 Banners = ItemRarityID.Blue;
        public const System.Int32 DungeonLoot = ItemRarityID.Green;
        public const System.Int32 JungleLoot = ItemRarityID.Green;
        public const System.Int32 EarlyHardmodeMaterial = ItemRarityID.Orange;
        public const System.Int32 HardDungeonLoot = ItemRarityID.Yellow;
        #endregion
    }

    public class Price {
        public static System.Int32 SkyMerchantShopItem { get; set; } = Item.sellPrice(gold: 1);
        public static System.Int32 SkyMerchantCustomPurchasePrice { get; set; } = Item.buyPrice(gold: 5);
        public static System.Int32 PollutedOceanLoot { get; set; } = Item.sellPrice(silver: 50);
        public static System.Int32 CrabsonLoot { get; set; } = Item.sellPrice(silver: 75);
        public static System.Int32 GlimmerLoot { get; set; } = Item.sellPrice(gold: 1);
        public static System.Int32 DemonSiegeLoot { get; set; } = Item.sellPrice(gold: 2);
        public static System.Int32 OmegaStariteLoot { get; set; } = Item.sellPrice(gold: 3);
        public static System.Int32 SpaceStormLoot { get; set; } = Item.sellPrice(gold: 2, silver: 50);
        public static System.Int32 DustDevilLoot { get; set; } = Item.sellPrice(gold: 3);
        public static System.Int32 DemonSiegeTier2Loot { get; set; } = Item.sellPrice(gold: 8);
        public static System.Int32 UpriserLoot { get; set; } = Item.sellPrice(gold: 8, silver: 50);
        public static System.Int32 YinYangLoot { get; set; } = Item.sellPrice(gold: 10);
    }
}