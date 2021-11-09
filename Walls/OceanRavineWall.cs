using AQMod.Items.Placeable.Wall;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Walls
{
    public class OceanRavineWall : ModWall
    {
        public static Color GetColor(float value = 0f)
        {
            return new Color(128, 128, 128, 10) * ((float)Math.Sin((Main.GlobalTime + value * 0.35f) * 2f) * 0.2f + 0.8f);
        }

        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            dustType = 209;
            drop = ModContent.ItemType<MoonlightWall>();
            AddMapEntry(new Color(12, 12, 48));
        }
    }
}