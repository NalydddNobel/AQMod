using Aequus.Common;
using Aequus.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.CarpenterBounties
{
    public class FountainBounty : CarpenterBounty
    {
        public override bool CheckConditions(TileMapCache map, out string message, NPC carpenter = null)
        {
            message = "";
            //AequusHelpers.dustDebug(new Rectangle(rect.X * 16, rect.Y * 16, rect.Width * 16, rect.Height * 16), DustID.FrostHydra);
            if (!FindWaterfallsPass(map, out var waterfalls))
            {
                return false;
            }
            var surroundingRectangle = new Rectangle(map.Width, map.Height, 1, 1);
            foreach (var w in waterfalls)
            {
                //AequusHelpers.dustDebug(rect.X + w.X, rect.Y + w.Y, DustID.CursedTorch);
                if (surroundingRectangle.X > w.X)
                {
                    surroundingRectangle.X = w.X;
                }
                if (surroundingRectangle.Y > w.Y)
                {
                    surroundingRectangle.Y = w.Y;
                }
            }
            foreach (var w in waterfalls)
            {
                if (surroundingRectangle.X < w.X)
                {
                    int width = w.X - surroundingRectangle.X + 1;
                    if (width > surroundingRectangle.Width)
                        surroundingRectangle.Width = width;
                }
                if (surroundingRectangle.Y < w.Y)
                {
                    int height = w.Y - surroundingRectangle.Y + 1;
                    if (height > surroundingRectangle.Height)
                        surroundingRectangle.Height = height;
                }
            }
            surroundingRectangle.X--;
            surroundingRectangle.Width += 2;
            surroundingRectangle.Y--;
            surroundingRectangle.Height += 2;

            //var debugRect = surroundingRectangle;
            //debugRect.X += rect.X;
            //debugRect.Y += rect.Y;
            //debugRect.X *= 16;
            //debugRect.Y *= 16;
            //debugRect.Width *= 16;
            //debugRect.Height *= 16;
            //AequusHelpers.dustDebug(debugRect);

            FindCraftableTiles(map, surroundingRectangle, out var tiles);
            //foreach (var t in tiles)
            //{
            //    AequusHelpers.dustDebug(rect.X + t.X, rect.Y + t.Y, DustID.PurpleCrystalShard);
            //}

            if (tiles.Count == 0)
            {
                return false;
            }

            surroundingRectangle = new Rectangle(map.Width, map.Height, 1, 1);
            foreach (var t in tiles)
            {
                if (surroundingRectangle.X > t.X)
                {
                    surroundingRectangle.X = t.X;
                }
                if (surroundingRectangle.Y > t.Y)
                {
                    surroundingRectangle.Y = t.Y;
                }
            }
            foreach (var t in tiles)
            {
                if (surroundingRectangle.X < t.X)
                {
                    int width = t.X - surroundingRectangle.X + 1;
                    if (width > surroundingRectangle.Width)
                        surroundingRectangle.Width = width;
                }
                if (surroundingRectangle.Y < t.Y)
                {
                    int height = t.Y - surroundingRectangle.Y + 1;
                    if (height > surroundingRectangle.Height)
                        surroundingRectangle.Height = height;
                }
            }

            //debugRect = surroundingRectangle;
            //debugRect.X += rect.X;
            //debugRect.Y += rect.Y;
            //debugRect.X *= 16;
            //debugRect.Y *= 16;
            //debugRect.Width *= 16;
            //debugRect.Height *= 16;

            //AequusHelpers.dustDebug(debugRect, DustID.PurpleCrystalShard);

            int middleWidth = (int)Math.Round(surroundingRectangle.Width / 2f);
            for (int i = 0; i < middleWidth; i++)
            {
                for (int j = 0; j < surroundingRectangle.Height; j++)
                {
                    var point1 = new Point(surroundingRectangle.X + i, surroundingRectangle.Y + j);
                    var point2 = new Point(surroundingRectangle.X + surroundingRectangle.Width - 1 - i, surroundingRectangle.Y + j);
                    if (tiles.Contains(point1) || tiles.Contains(point2))
                    {
                        //if (map[point1].TileType == map[point2].TileType)
                        //{
                        //    AequusHelpers.dustDebug(rect.X + point1.X, rect.Y + point1.Y, DustID.CursedTorch);
                        //    AequusHelpers.dustDebug(rect.X + point2.X, rect.Y + point2.Y, DustID.CursedTorch);
                        //}
                        //else
                        //{
                        //    AequusHelpers.dustDebug(rect.X + point1.X, rect.Y + point1.Y, DustID.Clentaminator_Red);
                        //    AequusHelpers.dustDebug(rect.X + point2.X, rect.Y + point2.Y, DustID.Clentaminator_Red);
                        //}
                        if (map[point1].TileType != map[point2].TileType)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        private bool FindWaterfallsPass(TileMapCache map, out List<Point> waterfalls)
        {
            waterfalls = new List<Point>();
            int x = map.Area.X;
            int y = map.Area.Y;
            for (int i = 0; i < map.Area.Width; i++)
            {
                for (int j = 0; j < map.Area.Height; j++)
                {
                    var checkPoint = new Point(i, j);
                    var tile = map.Get(checkPoint);
                    if (tile.Liquid.Amount > 0)
                    {
                        for (int k = -1; k <= 1; k += 2)
                        {
                            bool successfullyGotWaterfalls = false;
                            if (map.InMap(i + k, j) && map[i + k, j].IsHalfBlock)
                            {
                                int waterfallX = i + k;
                                int waterfallY = j;
                                int dir = k;
                                for (int l = 0; l < 80; l++)
                                {
                                    if ((map[waterfallX, waterfallY].HasTile && map[waterfallX, waterfallY].IsHalfBlock) || (map.InMap(waterfallX, waterfallY + 1) && map[waterfallX, waterfallY + 1].IsFullySolid))
                                    {
                                        if (map.InMap(waterfallX + dir, waterfallY) && map[waterfallX + dir, waterfallY].IsFullySolid && !map[waterfallX + dir, waterfallY].IsHalfBlock)
                                        {
                                            dir = -dir;
                                        }
                                        waterfallX += dir;
                                    }
                                    else
                                    {
                                        waterfallY++;
                                    }
                                    if (!map.InMap(waterfallX, waterfallY))
                                    {
                                        break;
                                    }
                                    if (waterfalls.Contains(new Point(waterfallX, waterfallY)))
                                    {
                                        continue;
                                    }
                                    //AequusHelpers.dustDebug(x + waterfallX, y + waterfallY, DustID.CursedTorch);
                                    successfullyGotWaterfalls = true;
                                    waterfalls.Add(new Point(waterfallX, waterfallY));
                                }
                            }
                            if (successfullyGotWaterfalls)
                            {
                                waterfalls.Add(checkPoint);
                                waterfalls.Add(new Point(i + k, j));
                                AddWaterPool2(map, waterfalls, checkPoint);
                            }
                        }
                    }
                }
            }
            return waterfalls.Count > 0;
        }

        private void AddWaterPool2(TileMapCache map, List<Point> waterfalls, Point checkPoint)
        {
            var addPoints = new List<Point>();
            var checkedPoints = new List<Point>() { checkPoint };
            var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
            for (int k = 0; k < 1000; k++)
            {
                checkedPoints.AddRange(addPoints);
                addPoints.Clear();
                bool addedAny = false;
                if (checkedPoints.Count > 1000)
                {
                    waterfalls.AddRange(checkedPoints);
                    return;
                }
                for (int l = 0; l < checkedPoints.Count; l++)
                {
                    for (int m = 0; m < offsets.Length; m++)
                    {
                        var newPoint = new Point(checkedPoints[l].X + offsets[m].X, checkedPoints[l].Y + offsets[m].Y);
                        if (map.InMap(newPoint.X, newPoint.Y) && !checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) &&
                            map[newPoint].LiquidAmount > 0 && !map[newPoint].IsFullySolid)
                        {
                            addPoints.Add(newPoint);
                            addedAny = true;
                        }
                    }
                }
                if (!addedAny)
                {
                    waterfalls.AddRange(checkedPoints);
                    return;
                }
            }
            waterfalls.AddRange(checkedPoints);
        }

        private void FindCraftableTiles(TileMapCache map, Rectangle surroundingRectangle, out List<Point> tiles)
        {
            tiles = new List<Point>();
            for (int i = surroundingRectangle.X; i < surroundingRectangle.X + surroundingRectangle.Width; i++)
            {
                for (int j = surroundingRectangle.Y; j < surroundingRectangle.Y + surroundingRectangle.Height; j++)
                {
                    if (map.InMap(i, j) && map[i, j].IsFullySolid && CarpenterSystem.IsTileIDCraftable(map[i, j].TileType))
                    {
                        tiles.Add(new Point(i, j));
                    }
                }
            }
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults(ItemID.BottomlessBucket);
        }
    }
}