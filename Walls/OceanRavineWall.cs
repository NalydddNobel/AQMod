using AQMod.Items.Placeable.Wall;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Walls
{
    public class OceanRavineWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            dustType = 209;
            drop = ModContent.ItemType<Items.Placeable.Wall.OceanRavineWall>();
            AddMapEntry(new Color(12, 12, 48));
        }
    }
}