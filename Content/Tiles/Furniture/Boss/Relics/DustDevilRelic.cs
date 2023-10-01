﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Furniture.Boss.Relics {
    public class DustDevilRelic : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<BossRelicsTile>(), BossRelicsTile.DustDevil);
            Item.width = 30;
            Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.GetGlobalItem<AequusItem>().itemGravityCheck = 255;
        }
    }
}