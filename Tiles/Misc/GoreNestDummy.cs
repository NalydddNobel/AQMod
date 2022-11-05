using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc
{
    public class GoreNestDummy : ModTile
    {
        public override string Texture => AequusHelpers.GetPath<GoreNestTile>();

        public override void SetStaticDefaults()
        {
            AdjTiles = new int[] { TileID.DemonAltar };
            AddMapEntry(new Color(175, 15, 15), CreateMapEntryName("GoreNest"));
        }
    }
}