using Aequus.Graphics.RenderTargets;
using Aequus.Items;
using Aequus.Items.Tools.Misc;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Content.CarpenterBounties
{
    public class PirateShipBounty : CarpenterBounty
    {
        public override bool CheckConditions(ConditionInfo info, out string message)
        {
            int padding = ShutterstockerSceneRenderer.TilePaddingForChecking / 2;
            var housingWalls = new Dictionary<Point, List<Point>>();
            for (int i = padding; i < info.Width - padding; i++)
            {
                for (int j = padding; j < info.Height - padding; j++)
                {
                    if (info[i, j].IsFullySolid || info[i, j].WallType == WallID.None || !Main.wallHouse[info[i, j].WallType] || info[i, j].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor))
                    {
                        continue;
                    }

                    foreach (var l in housingWalls.Values)
                    {
                        if (l.Contains(new Point(i, j)))
                            goto Continue;
                    }

                    var pendingList = CarpenterSystem.FindWallTiles(info.Map, i, j);
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
            int waterLine = GetWaterLine(info.Map, housingWalls);
            if (waterLine < 5)
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.NotEnoughWater");
                return false;
            }
            int decorCount = CountDecorInsideHouse(info.Map, housingWalls);
            if (decorCount < 15)
            {
                message = Language.GetTextValueWith(LanguageKey + ".Reply.NotEnoughFurniture", new { FurnitureAmt = decorCount });
                return false;
            }
            message = Language.GetTextValue(LanguageKey + ".Reply.Completed");
            return true;
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
            return AequusItem.SetDefaults<WhiteFlag>();
        }
    }
}