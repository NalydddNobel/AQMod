using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Aequus.Content.Carpentery.Bounties.Steps
{
    public class WaterfallSearchStep : Step
    {
        public class WaterfallInterest : StepInterest
        {
            public List<Point> waterfalls;
            public Rectangle resultRectangle;

            public override void CompileInterestingPoints(StepInfo info)
            {
                waterfalls = new List<Point>();
                var map = info.Map;
                int x = map.Area.X;
                int y = map.Area.Y;
                for (int i = 0; i < map.Area.Width; i++)
                {
                    for (int j = 0; j < map.Area.Height; j++)
                    {
                        var checkPoint = new Point(i, j);
                        var tile = map.Get(checkPoint);
                        if (tile.LiquidAmount > 0)
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
                                        if (map[waterfallX, waterfallY].HasTile && map[waterfallX, waterfallY].IsHalfBlock || map.InSceneRenderedMap(waterfallX, waterfallY + 1) && map[waterfallX, waterfallY + 1].IsFullySolid)
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
                                    AddWaterPool(info, checkPoint);
                                }
                            }
                        }
                    }
                }
                resultRectangle = CarpenterBounty.GetSurroundings(info, waterfalls);
            }

            public void AddWaterPool(StepInfo info, Point checkPoint)
            {
                var map = info.Map;
                var addPoints = new List<Point>();
                var checkedPoints = new List<Point>() { checkPoint };
                var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
                for (int k = 0; k < 500; k++)
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
        }

        public int LiquidWanted;

        public WaterfallSearchStep(int liquidWanted) : base()
        {
            LiquidWanted = liquidWanted;
        }

        protected override void Init(StepInfo info)
        {
            info.AddInterest(new WaterfallInterest());
        }

        protected override StepResult ProvideResult(StepInfo info)
        {
            var waterfalls = info.GetInterest<WaterfallInterest>();
            waterfalls.Update(info);
            return new StepResult("NoWaterfalls")
            {
                success = waterfalls.waterfalls.Count > 0 && waterfalls.waterfalls.ContainsAny((p) => info[p].LiquidType == LiquidWanted)
            };
        }
    }
}