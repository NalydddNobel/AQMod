﻿using Terraria;
using Terraria.ID;

namespace Aequus.Common.Items {
    public static class ItemCommons {
        public class Rarity {
            public static int SkyMerchantShopItem = ItemRarityID.Blue;
            public static int CrabCreviceLoot = ItemRarityID.Blue;
            public static int GlimmerLoot = ItemRarityID.Green;
            public static int DemonSiegeTier1Loot = ItemRarityID.Orange;
            public static int OmegaStariteLoot = ItemRarityID.LightRed;
            public static int SpaceStormLoot = ItemRarityID.Pink;
            public static int DustDevilLoot = ItemRarityID.LightPurple;
            public static int DemonSiegeTier2Loot = ItemRarityID.Yellow;
            public static int UpriserLoot = ItemRarityID.Red;
            public static int YinYangLoot = ItemRarityID.Red;

            #region Vanilla
            public const int shimmerPermaPowerup = ItemRarityID.LightPurple;
            public const int banners = ItemRarityID.Blue;
            public const int bossMasks = ItemRarityID.Blue;
            public const int dungeonLoot = ItemRarityID.Green;
            public const int jungleLoot = ItemRarityID.Green;
            #endregion
        }

        public class Price {
            public static int SkyMerchantShopItem = Item.sellPrice(gold: 1);
            public static int SkyMerchantCustomPurchasePrice = Item.buyPrice(gold: 10);
            public static int PollutedOceanLoot = Item.buyPrice(silver: 50);
            public static int GlimmerLoot = Item.buyPrice(gold: 1);
            public static int DemonSiegeLoot = Item.buyPrice(gold: 2);
            public static int OmegaStariteLoot = Item.buyPrice(gold: 3);
            public static int SpaceStormLoot = Item.buyPrice(gold: 2, silver: 50);
            public static int DustDevilLoot = Item.buyPrice(gold: 3);
            public static int DemonSiegeTier2Loot = Item.buyPrice(gold: 8);
            public static int UpriserLoot = Item.buyPrice(gold: 8, silver: 50);
            public static int YinYangLoot = Item.buyPrice(gold: 10);
        }
    }
}