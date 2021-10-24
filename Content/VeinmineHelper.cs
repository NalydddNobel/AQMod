using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AQMod.Content
{
    public static class VeinmineHelper
    {
        public static byte VeinmineReps { get; set; } = 10;

        public static bool IsSilt(int i, int j)
        {
            return IsSilt(Main.tile[i, j].type);
        }

        public static bool IsSilt(Tile tile)
        {
            return IsSilt(tile.type);
        }

        public static bool IsSilt(int type)
        {
            return type == TileID.Silt || type == TileID.Slush || type == TileID.DesertFossil;
        }

        public static bool CanVeinmineAtAll(int i, int j)
        {
            return CanVeinmineAtAll(Main.tile[i, j].type);
        }

        public static bool CanVeinmineAtAll(Tile tile)
        {
            return CanVeinmineAtAll(tile.type);
        }

        public static bool CanVeinmineAtAll(int type)
        {
            return !Main.tileFrameImportant[type];
        }

        public static void VeinmineTile(int i, int j, Player player)
        {
            int type = Main.tile[i, j].type;
            List<Point> points = new List<Point>() { new Point(i, j) };
            expand(ref points, i, j, type);
            for (int k = 0; k < VeinmineReps; k++)
            {
                if (points.Count == 0)
                    break;
                foreach (var p in points)
                {
                    if (Main.tile[p.X, p.Y].active())
                        WorldGen.KillTile(p.X, p.Y);
                }
                var oldPoints = new List<Point>(points);
                points.Clear();
                foreach (var p in oldPoints)
                {
                    expand(ref points, p.X, p.Y, type);
                }
            }
        }

        private static void expand(ref List<Point> points, int x, int y, int type)
        {
            expandQuarry(ref points, x, y - 1, type);
            expandQuarry(ref points, x + 1, y - 1, type);
            expandQuarry(ref points, x + 1, y, type);
            expandQuarry(ref points, x + 1, y + 1, type);
            expandQuarry(ref points, x, y + 1, type);
            expandQuarry(ref points, x - 1, y + 1, type);
            expandQuarry(ref points, x - 1, y, type);
            expandQuarry(ref points, x - 1, y - 1, type);
        }

        private static void expandQuarry(ref List<Point> points, int x, int y, int type)
        {
            if (Framing.GetTileSafely(x, y).active() && Main.tile[x, y].type == type)
                points.Add(new Point(x, y));
        }
    }
}