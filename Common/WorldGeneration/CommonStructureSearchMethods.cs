using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.WorldGeneration
{
    internal static class CommonStructureSearchMethods
    {
        public static bool LihzahrdAltar(out Point position)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    var tile = Framing.GetTileSafely(i, j);
                    if (tile.active() && tile.type == TileID.LihzahrdAltar)
                    {
                        position = new Point(i, j);
                        return true;
                    }
                }
            }
            position = default(Point);
            return false;
        }
    }
}
