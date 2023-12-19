namespace Aequus.Common.Items;

public static class ItemCommons {
    public class Rarity {
        public static int SkyMerchantShopItem = ItemRarityID.Blue;
        public static int PollutedOceanLoot = ItemRarityID.Blue;
        public static int CrabsonLoot = ItemRarityID.Blue;
        public static int GlimmerLoot = ItemRarityID.Green;
        public static int DemonSiegeTier1Loot = ItemRarityID.Orange;
        public static int OmegaStariteLoot = ItemRarityID.LightRed;
        public static int SpaceStormLoot = ItemRarityID.Pink;
        public static int DustDevilLoot = ItemRarityID.LightPurple;
        public static int DemonSiegeTier2Loot = ItemRarityID.Yellow;
        public static int UpriserLoot = ItemRarityID.Red;
        public static int YinYangLoot = ItemRarityID.Red;

        #region Vanilla
        public const int ShimmerPermaPowerup = ItemRarityID.LightPurple;
        public const int Banners = ItemRarityID.Blue;
        public const int BossMasks = ItemRarityID.Blue;
        public const int DungeonLoot = ItemRarityID.Green;
        public const int JungleLoot = ItemRarityID.Green;
        public const int EarlyHardmodeMaterial = ItemRarityID.Orange;
        public const int HardDungeonLoot = ItemRarityID.Yellow;
        #endregion
    }

    public class Price {
        public static int SkyMerchantShopItem = Item.sellPrice(gold: 1);
        public static int SkyMerchantCustomPurchasePrice = Item.buyPrice(gold: 5);
        public static int PollutedOceanLoot = Item.sellPrice(silver: 50);
        public static int CrabsonLoot = Item.sellPrice(silver: 75);
        public static int GlimmerLoot = Item.sellPrice(gold: 1);
        public static int DemonSiegeLoot = Item.sellPrice(gold: 2);
        public static int OmegaStariteLoot = Item.sellPrice(gold: 3);
        public static int SpaceStormLoot = Item.sellPrice(gold: 2, silver: 50);
        public static int DustDevilLoot = Item.sellPrice(gold: 3);
        public static int DemonSiegeTier2Loot = Item.sellPrice(gold: 8);
        public static int UpriserLoot = Item.sellPrice(gold: 8, silver: 50);
        public static int YinYangLoot = Item.sellPrice(gold: 10);
    }
}