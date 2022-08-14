using Aequus.Common;
using Aequus.Graphics.RenderTargets;
using Aequus.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.CarpenterBounties
{
    public class PirateShipBounty : CarpenterBounty
    {
        public override bool CheckConditions(TileMapCache map, out string message, NPC carpenter = null)
        {
            int padding = ShutterstockerSceneRenderer.TilePaddingForChecking / 2;
            var housingWalls = new Dictionary<Point, List<Point>>();
            for (int i = 0; i < Main.maxDust; i++)
            {
                Main.dust[i].active = false;
            }
            for (int i = padding; i < map.Width - padding; i++)
            {
                for (int j = padding; j < map.Height - padding; j++)
                {
                    if (map[i, j].IsFullySolid || map[i, j].WallType == WallID.None || !Main.wallHouse[map[i, j].WallType] || map[i, j].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor))
                    {
                        continue;
                    }

                    foreach (var l in housingWalls.Values)
                    {
                        if (l.Contains(new Point(i, j)))
                            goto Continue;
                    }

                    var pendingList = FindWallTiles(map, i, j);
                    if (pendingList.Count < 30)
                        continue;
                    housingWalls.Add(new Point(i, j), pendingList);

                Continue:
                    continue;
                }
            }

            if (housingWalls.Count < 2)
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.NotEnoughRooms");
                return false;
            }
            int waterLine = GetWaterLine(map, housingWalls);
            if (waterLine < 5)
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.NotEnoughWater");
                return false;
            }
            int decorCount = CountDecorInsideHouse(map, housingWalls);
            if (decorCount < 15)
            {
                message = Language.GetTextValueWith(LanguageKey + ".Reply.NotEnoughFurniture", new { FurnitureAmt = decorCount });
                return false;
            }
            message = Language.GetTextValue(LanguageKey + ".Reply.Completed");
            return true;
        }

        public static List<Point> FindWallTiles(TileMapCache map, int x, int y)
        {
            var addPoints = new List<Point>();
            var checkedPoints = new List<Point>() { new Point(x, y) };
            var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
            for (int k = 0; k < 1000; k++)
            {
                checkedPoints.AddRange(addPoints);
                addPoints.Clear();
                bool addedAny = false;
                if (checkedPoints.Count > 1000)
                {
                    return checkedPoints;
                }
                for (int l = 0; l < checkedPoints.Count; l++)
                {
                    for (int m = 0; m < offsets.Length; m++)
                    {
                        var newPoint = new Point(checkedPoints[l].X + offsets[m].X, checkedPoints[l].Y + offsets[m].Y);
                        if (map.InSceneRenderedMap(newPoint.X, newPoint.Y) && !checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) && map[newPoint].WallType != WallID.None && Main.wallHouse[map[newPoint].WallType])
                        {
                            var slopeType = SlopeType.Solid;
                            if (offsets[m].X != 0)
                            {
                                slopeType = map[newPoint].Slope;
                                if (TileID.Sets.Platforms[map[newPoint].TileType])
                                {
                                    if (map[newPoint].TileFrameX == 144)
                                    {
                                        slopeType = SlopeType.SlopeDownLeft;
                                    }
                                    if (map[newPoint].TileFrameX == 180)
                                    {
                                        slopeType = SlopeType.SlopeDownRight;
                                    }
                                }
                            }

                            if (!map[newPoint].HasTile || ((!map[newPoint].IsSolid || map[newPoint].IsSolidTop) && (!map[newPoint].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor) || slopeType != SlopeType.Solid)))
                            {
                                for (int n = 0; n < offsets.Length; n++)
                                {
                                    var checkWallPoint = newPoint + offsets[n];
                                    if (!map.InSceneRenderedMap(checkWallPoint) || (!map[checkWallPoint].IsFullySolid && (map[checkWallPoint].WallType == WallID.None || !Main.wallHouse[map[checkWallPoint].WallType])))
                                    {
                                        return new List<Point>() { new Point(x, y) };
                                    }
                                }
                                addPoints.Add(newPoint);
                                addedAny = true;
                            }
                        }
                    }
                }
                if (!addedAny)
                {
                    return checkedPoints;
                }
            }
            return checkedPoints;
        }

        public static int GetWaterLine(TileMapCache map, Dictionary<Point, List<Point>> houses)
        {
            int waterY = 1000;
            foreach (var h in houses.Values)
            {
                foreach (var p in h)
                {
                    if (p.Y < waterY)
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            var checkPoint = new Point(p.X + i, p.Y);
                            if (!map.InSceneRenderedMap(checkPoint) || checkPoint.Y >= waterY)
                            {
                                break;
                            }
                            if (map[checkPoint].LiquidAmount > 0)
                            {
                                waterY = checkPoint.Y;
                                break;
                            }
                            if (!map[checkPoint].IsFullySolid && !h.Contains(checkPoint))
                            {
                                break;
                            }
                        }

                        for (int i = 0; i < 1000; i++)
                        {
                            var checkPoint = new Point(p.X - i, p.Y);
                            if (!map.InSceneRenderedMap(checkPoint) || checkPoint.Y >= waterY)
                            {
                                break;
                            }
                            if (map[checkPoint].LiquidAmount > 0)
                            {
                                waterY = checkPoint.Y;
                                break;
                            }
                            if (!map[checkPoint].IsFullySolid && !h.Contains(checkPoint))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (waterY == 1000)
                return 0;
            var cap = GetRectangleCapture(houses);
            return (cap.Y + cap.Height) - waterY + 1;
        }

        public static Rectangle GetRectangleCapture(Dictionary<Point, List<Point>> houses)
        {
            int startX = int.MaxValue;
            int startY = int.MaxValue;
            int endX = 0;
            int endY = 0;

            foreach (var h in houses.Values)
            {
                foreach (var p in h)
                {
                    if (p.X < startX)
                    {
                        startX = p.X;
                    }
                    if (p.Y < startY)
                    {
                        startY = p.Y;
                    }
                    if (p.X > endX)
                    {
                        endX = p.X;
                    }
                    if (p.Y > endY)
                    {
                        endY = p.Y;
                    }
                }
            }

            return new Rectangle(startX, startY, endX - startX, endY - startY);
        }

        public static int CountDecorInsideHouse(TileMapCache map, Dictionary<Point, List<Point>> houses)
        {
            int decorAmt = 0;
            var tileStyleData = new Dictionary<int, List<int>>();

            foreach (var h in houses.Values)
            {
                foreach (var p in h)
                {
                    if (map[p].HasTile)
                    {
                        if (!map[p].IsSolid)
                        {
                            int style = AequusHelpers.GetTileStyle(map[p].TileType, map[p].TileFrameX, map[p].TileFrameY);
                            if (tileStyleData.TryGetValue(map[p].TileType, out List<int> compareStyle))
                            {
                                if (compareStyle.Contains(style))
                                {
                                    continue;
                                }
                                compareStyle.Add(style);
                            }
                            else
                            {
                                tileStyleData.Add(map[p].TileType, new List<int>() { style });
                            }

                            decorAmt++;
                        }
                    }
                }
            }
            return decorAmt;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults(ItemID.PirateHat);
        }
    }
}