using Aequus.Items.Placeable.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
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