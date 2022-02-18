using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AQMod.Tiles.Walls
{
    public class OceanRavineWall : ModWall
    {
        public override void SetDefaults()
        {
            dustType = 209;
            drop = ModContent.ItemType<Items.Placeable.Wall.OceanRavineWall>();
            AddMapEntry(new Color(12, 12, 48));
        }
    }
}