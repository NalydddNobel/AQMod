﻿using Aequus.Common.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Banners.Items {
    public class TrapSkeletonBanner : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<MonsterBanners>(), MonsterBanners.TrapSkeletonBanner);
            Item.rare = ItemDefaults.RarityBanner;
            Item.value = Item.sellPrice(silver: 2);
        }
    }
}