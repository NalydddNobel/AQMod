using Aequus.Items.Placeable.Crab;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Tiles.Crab
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