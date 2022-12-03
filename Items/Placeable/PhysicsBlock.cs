﻿using Aequus.Content.AnalysisQuests;
using Aequus.Tiles.PhysicistBlocks;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public class PhysicsBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PhysicsBlockTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
}