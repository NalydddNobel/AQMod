namespace Aequus.Common.Items;

public static class ItemCommons {
    public class Rarity {
        /// <summary><see cref="ItemRarityID.Blue"/></summary>
        public static int SkyMerchantShopItem { get; set; } = ItemRarityID.Blue;

        /// <summary><see cref="ItemRarityID.Blue"/></summary>
        public static int PollutedOceanLoot { get; set; } = ItemRarityID.Blue;

        /// <summary><see cref="ItemRarityID.Blue"/></summary>
        public static int AquaticBossLoot { get; set; } = ItemRarityID.Blue;

        /// <summary><see cref="ItemRarityID.LightPurple"/></summary>
        public static int JungleLoot { get; set; } = ItemRarityID.Green;

        /// <summary><see cref="ItemRarityID.Green"/></summary>
        public static int GlimmerLoot { get; set; } = ItemRarityID.Green;

        /// <summary><see cref="ItemRarityID.Green"/></summary>
        public static int DungeonLoot { get; set; } = ItemRarityID.Green;

        /// <summary><see cref="ItemRarityID.Orange"/></summary>
        public static int DemonSiegeTier1Loot { get; set; } = ItemRarityID.Orange;

        /// <summary><see cref="ItemRarityID.LightRed"/></summary>
        public static int OmegaStariteLoot { get; set; } = ItemRarityID.LightRed;

        /// <summary><see cref="ItemRarityID.Orange"/></summary>
        public static int EarlyHardmodeMaterial { get; set; } = ItemRarityID.Orange;

        /// <summary><see cref="ItemRarityID.LightRed"/></summary>
        public static int HardSandstormBlizzardLoot { get; set; } = ItemRarityID.LightRed;

        /// <summary><see cref="ItemRarityID.Pink"/></summary>
        public static int SpaceStormLoot { get; set; } = ItemRarityID.Pink;

        /// <summary><see cref="ItemRarityID.LightPurple"/></summary>
        public static int DustDevilLoot { get; set; } = ItemRarityID.LightPurple;

        /// <summary><see cref="ItemRarityID.Yellow"/></summary>
        public static int HardDungeonLoot { get; set; } = ItemRarityID.Yellow;

        /// <summary><see cref="ItemRarityID.Yellow"/></summary>
        public static int DemonSiegeTier2Loot { get; set; } = ItemRarityID.Yellow;

        /// <summary><see cref="ItemRarityID.Red"/></summary>
        public static int UpriserLoot { get; set; } = ItemRarityID.Red;

        /// <summary><see cref="ItemRarityID.Red"/></summary>
        public static int YinYangLoot { get; set; } = ItemRarityID.Red;

        /// <summary><see cref="ItemRarityID.LightPurple"/></summary>
        public static int ShimmerPermaPowerup { get; set; } = ItemRarityID.LightPurple;

        /// <summary><see cref="ItemRarityID.LightRed"/></summary>
        public static readonly int WallOfFleshLoot = ItemRarityID.LightRed;
        /// <summary><see cref="ItemRarityID.Yellow"/></summary>
        public static readonly int PlanteraLoot = ItemRarityID.Yellow;
        /// <summary><see cref="ItemRarityID.Red"/></summary>
        public static readonly int MoonLordLoot = ItemRarityID.Red;
    }

    public class Price {
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