using Terraria;

namespace Aequus
{
    partial class AequusHelpers
    {
        public static void Active(this Tile tile, bool value)
        {
            tile.HasTile = value;
        }
    }
}