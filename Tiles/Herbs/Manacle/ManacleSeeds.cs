﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Herbs.Manacle {
    public class ManacleSeeds : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<ManacleTile>());
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;
        }
    }
}