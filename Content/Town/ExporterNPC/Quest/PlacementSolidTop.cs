using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Town.ExporterNPC.Quest
{
    [Obsolete("Exporter thievery was removed.")]
    public class PlacementSolidTop : IPlacementData
    {
        public List<Point> ScanRoom(NPC townNPC)
        {
            var home = IPlacementData.GetStartingPoint(townNPC);
            var p = new List<Point>();

            InnerScanRoom(home, p, 1);
            InnerScanRoom(home, p, -1);

            return p;
        }
        public static void InnerScanRoom(Point home, List<Point> p, int dir)
        {
            int wallUp = 0;
            int wallDown = 0;
            int oldY = home.Y;
            for (int i = 0; i < 20; i++)
            {
                int x = home.X + i * dir;

                if (Main.tile[x, home.Y].IsSolid() || TileID.Sets.RoomNeeds.CountsAsDoor.ContainsAny(
                    (type) => type == Main.tile[x, home.Y].TileType))
                {
                    if (wallUp > 10 || Main.tile[x - dir, home.Y].IsSolid())
                    {
                        break;
                    }
                    home.Y++;
                    wallUp++;
                    i--;
                    continue;
                }
                wallUp = 0;

                if (!Main.tile[x, home.Y - 1].IsSolid())
                {
                    wallDown++;
                    home.Y--;
                    if (home.Y < 10 || wallDown > 10)
                    {
                        break;
                    }
                    i--;
                    continue;
                }
                wallDown = 0;

                InnerScanRoom_Downwards(x, home.Y, p);
            }
        }
        public static void InnerScanRoom_Downwards(int x, int y, List<Point> points)
        {
            for (int endY = y + 20; y < endY; y++)
            {
                if (Main.tile[x, y].HasTile)
                {
                    if (Main.tile[x, y].SolidTopType() && !Main.tile[x, y - 1].HasTile)
                    {
                        points.Add(new Point(x, y));
                    }
                    else if (Main.tile[x, y].SolidType())
                    {
                        break;
                    }
                }
            }
        }
    }
}