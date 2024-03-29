﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas2x2 {
    public class YinPainting : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings2x2>(), WallPaintings2x2.YinPainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}