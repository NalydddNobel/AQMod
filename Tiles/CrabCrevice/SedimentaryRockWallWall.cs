using Aequus.Items.Placeable.CrabCrevice;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice
{
    public class SedimentaryRockWallWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = 209;
            ItemDrop = ModContent.ItemType<SedimentaryRockWall>();
            AddMapEntry(new Color(70, 40, 20));
        }
    }
}