﻿using Aequus.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.BossTrophies
{
    public class CrabsonTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.Crabson);
            Item.maxStack = 99;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}