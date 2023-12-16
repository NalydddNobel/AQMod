using Terraria;
using Terraria.ID;

namespace Aequus.Common.Items;

public static class ItemDefaults {
    public class Rarity {
        public static int SkyMerchantShopItem { get; set; } = ItemRarityID.Blue;
        public static int PollutedOceanLoot { get; set; } = ItemRarityID.Blue;
        public static int CrabsonLoot { get; set; } = ItemRarityID.Blue;
        public static int GlimmerLoot { get; set; } = ItemRarityID.Green;
        public static int DemonSiegeTier1Loot { get; set; } = ItemRarityID.Orange;
        public static int OmegaStariteLoot { get; set; } = ItemRarityID.LightRed;
        public static int SpaceStormLoot { get; set; } = ItemRarityID.Pink;
        public static int DustDevilLoot { get; set; } = ItemRarityID.LightPurple;
        public static int DemonSiegeTier2Loot { get; set; } = ItemRarityID.Yellow;
        public static int UpriserLoot { get; set; } = ItemRarityID.Red;
        public static int YinYangLoot { get; set; } = ItemRarityID.Red;
        public static int AccessoryCrown { get; set; } = ItemRarityID.LightRed;

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
        public static int SkyMerchantShopItem { get; set; } = Item.sellPrice(gold: 1);
        public static int SkyMerchantCustomPurchasePrice { get; set; } = Item.buyPrice(gold: 5);
        public static int PollutedOceanLoot { get; set; } = Item.sellPrice(silver: 50);
        public static int CrabsonLoot { get; set; } = Item.sellPrice(silver: 75);
        public static int GlimmerLoot { get; set; } = Item.sellPrice(gold: 1);
        public static int DemonSiegeLoot { get; set; } = Item.sellPrice(gold: 2);
        public static int OmegaStariteLoot { get; set; } = Item.sellPrice(gold: 3);
        public static int SpaceStormLoot { get; set; } = Item.sellPrice(gold: 2, silver: 50);
        public static int DustDevilLoot { get; set; } = Item.sellPrice(gold: 3);
        public static int DemonSiegeTier2Loot { get; set; } = Item.sellPrice(gold: 8);
        public static int UpriserLoot { get; set; } = Item.sellPrice(gold: 8, silver: 50);
        public static int YinYangLoot { get; set; } = Item.sellPrice(gold: 10);
        public static int AccessoryCrown { get; set; } = Item.buyPrice(gold: 30);
    }
}