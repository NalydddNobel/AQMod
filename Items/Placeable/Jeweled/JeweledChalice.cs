﻿using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Jeweled
{
    public class JeweledChalice : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileID.Pots/*ModContent.TileType<JeweledChaliceTile>()*/);
            Item.width = 16;
            Item.height = 24;
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
            Item.maxStack = 99;
        }
    }
}