using AQMod.Tiles;
using Terraria;
using Terraria.ID;

namespace AQMod.Content.World.Generation
{
    internal sealed class MushroomBGWallGenerator
    {
        public static bool GenerateSpire(int x, int y, int width, bool checkTiles = false)
        {
            if (checkTiles)
            {
                for (; y < Main.maxTilesY - 300; y++)
                {
                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new Tile();
                        return false;
                    }
                    if (Main.tile[x, y].active() && Main.tile[x, y].type == TileID.MushroomGrass)
                    {
                        break;
                    }
                }
            }
            int endY = y;
            for (; endY > (int)Main.worldSurface + 50; endY--)
            {
                if (Main.tile[x, endY] == null)
                {
                    Main.tile[x, endY] = new Tile();
                    return false;
                }
                if (Main.tile[x, y].active() && Main.tile[x, endY].Solid())
                {
                    break;
                }
            }
            int w = width;
            int mid = y + (y - endY) / 2;
            for (; y > endY; y--)
            {
                int right = w / 2;
                int left = w - right - 1;
                for (int wallX = x - left; wallX < x + right; wallX++)
                {
                    Framing.GetTileSafely(wallX, y).wall = WallID.MushroomUnsafe;
                }
                if (WorldGen.genRand.NextBool(3))
                {
                    if (y < mid)
                    {
                        w--;
                    }
                    else
                    {
                        w++;
                    }
                }
            }
            return true;
        }
    }
}