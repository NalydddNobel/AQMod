using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Walls
{
    public class PetrifiedWoodWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            dustType = 209;
            drop = ModContent.ItemType<Items.Placeable.Wall.PetrifiedWoodWall>();
            AddMapEntry(new Color(70, 60, 48));
        }
    }
}