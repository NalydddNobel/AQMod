using Aequus.Common;
using Aequus.Items;
using Aequus.Items.Tools;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Content.CarpenterBounties
{
    public class FountainBounty : CarpenterBounty
    {
        public override bool CheckConditions(ConditionInfo info, out string message)
        {
            message = "";
            if (!FindWaterfallsPass(info.Map, out var waterfalls))
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.NoWaterfalls");
                return false;
            }
            var surroundingRectangle = new Rectangle(info.Width, info.Height, 1, 1);
            foreach (var w in waterfalls)
            {
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
            if (surroundingRectangle.Height < 7)
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.ShortWaterfalls");
                return false;
            }
            surroundingRectangle.X--;
            surroundingRectangle.Width += 2;
            surroundingRectangle.Y--;
            surroundingRectangle.Height += 2;

            FindCraftableTilesPass(info.Map, surroundingRectangle, out var tiles);

            if (tiles.Count == 0 || tiles.Count < 12)
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.NoCraftedBlocks");
                return false;
            }

            surroundingRectangle = new Rectangle(info.Width, info.Height, 1, 1);
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

            int middleWidth = (int)Math.Round(surroundingRectangle.Width / 2f);
            for (int i = 0; i < middleWidth; i++)
            {
                for (int j = 0; j < surroundingRectangle.Height; j++)
                {
                    var point1 = new Point(surroundingRectangle.X + i, surroundingRectangle.Y + j);
                    var point2 = new Point(surroundingRectangle.X + surroundingRectangle.Width - 1 - i, surroundingRectangle.Y + j);
                    if (tiles.Contains(point1) || tiles.Contains(point2))
                    {
                        if (info[point1].TileType != info[point2].TileType)
                        {
                            var misc = info[point1].Misc;
                            misc.TileColor = PaintID.DeepRedPaint;
                            info[point1] = new TileDataCache(info[point1].Type, info[point1].Liquid, misc, info[point1].Wall, info[point1].Aequus);

                            misc = info[point2].Misc;
                            misc.TileColor = PaintID.DeepRedPaint;
                            info[point2] = new TileDataCache(info[point2].Type, info[point2].Liquid, misc, info[point2].Wall, info[point1].Aequus);

                            message = Language.GetTextValue(LanguageKey + ".Reply.NotSymmetric");
                            return false;
                        }
                    }
                }
            }

            message = Language.GetTextValue(LanguageKey + ".Reply.Completed");
            return true;
        }

        public bool FindWaterfallsPass(TileMapCache map, out List<Point> waterfalls)
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
                            if (map.InSceneRenderedMap(i + k, j) && map[i + k, j].IsHalfBlock)
                            {
                                int waterfallX = i + k;
                                int waterfallY = j;
                                int dir = k;
                                for (int l = 0; l < 80; l++)
                                {
                                    if ((map[waterfallX, waterfallY].HasTile && map[waterfallX, waterfallY].IsHalfBlock) || (map.InSceneRenderedMap(waterfallX, waterfallY + 1) && map[waterfallX, waterfallY + 1].IsFullySolid))
                                    {
                                        if (map.InSceneRenderedMap(waterfallX + dir, waterfallY) && map[waterfallX + dir, waterfallY].IsFullySolid && !map[waterfallX + dir, waterfallY].IsHalfBlock)
                                        {
                                            dir = -dir;
                                        }
                                        waterfallX += dir;
                                    }
                                    else
                                    {
                                        waterfallY++;
                                    }
                                    if (!map.InSceneRenderedMap(waterfallX, waterfallY))
                                    {
                                        break;
                                    }
                                    if (waterfalls.Contains(new Point(waterfallX, waterfallY)))
                                    {
                                        continue;
                                    }
                                    successfullyGotWaterfalls = true;
                                    waterfalls.Add(new Point(waterfallX, waterfallY));
                                }
                            }
                            if (successfullyGotWaterfalls)
                            {
                                waterfalls.Add(checkPoint);
                                waterfalls.Add(new Point(i + k, j));
                                AddWaterPool(map, waterfalls, checkPoint);
                            }
                        }
                    }
                }
            }
            return waterfalls.Count > 0;
        }
        public void AddWaterPool(TileMapCache map, List<Point> waterfalls, Point checkPoint)
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
                        if (map.InSceneRenderedMap(newPoint.X, newPoint.Y) && !checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) &&
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

        public void FindCraftableTilesPass(TileMapCache map, Rectangle surroundingRectangle, out List<Point> tiles)
        {
            tiles = new List<Point>();
            for (int i = surroundingRectangle.X; i < surroundingRectangle.X + surroundingRectangle.Width; i++)
            {
                for (int j = surroundingRectangle.Y; j < surroundingRectangle.Y + surroundingRectangle.Height; j++)
                {
                    if (map.InSceneRenderedMap(i, j) && map[i, j].IsFullySolid && CarpenterSystem.IsTileIDCraftable(map[i, j].TileType))
                    {
                        tiles.Add(new Point(i, j));
                    }
                }
            }
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults<AdvancedRuler>();
        }
    }
}