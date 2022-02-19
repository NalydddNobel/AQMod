using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.WorldGeneration
{
    internal static class CommonStructureSearchMethods
    {
        public static bool LihzahrdAltar(out Point altarLocation, bool useMapData = false)
        {
            if (useMapData)
            {
                for (int i = 0; i < Main.Map.MaxWidth; i++)
                {
                    for (int j = 0; j < Main.Map.MaxHeight; j++)
                    {
                        if (Main.Map[i, j].Type == TileID.LihzahrdAltar)
                        {
                            altarLocation = new Point(i, j);
                            return true;
                        }
                    }
                }
            }
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    var tile = Framing.GetTileSafely(i, j);
                    if (tile.active() && tile.type == TileID.LihzahrdAltar)
                    {
                        altarLocation = new Point(i, j);
                        return true;
                    }
                }
            }
            altarLocation = default(Point);
            return false;
        }
    }
}
