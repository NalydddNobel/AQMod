using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties.Steps
{
    public class SymmetricHorizontalStep : Step
    {
        public class Interest : StepInterest
        {
            public List<Point> symmetricTiles;
            public List<Point> nonSymmetricTiles;

            public List<Point> givenPoints;
            public Rectangle givenRectangle;

            public override void CompileInterestingPoints(StepInfo info)
            {
                symmetricTiles = new List<Point>();
                nonSymmetricTiles = new List<Point>();
                int middleWidth = (int)Math.Round(givenRectangle.Width / 2f);
                for (int i = 0; i < middleWidth; i++)
                {
                    for (int j = 0; j < givenRectangle.Height; j++)
                    {
                        var point1 = new Point(givenRectangle.X + i, givenRectangle.Y + j);
                        var point2 = new Point(givenRectangle.X + givenRectangle.Width - i, givenRectangle.Y + j);
                        if (givenPoints.Contains(point1) || givenPoints.Contains(point2))
                        {
                            if (info[point1].TileType != info[point2].TileType)
                            {
                                nonSymmetricTiles.Add(point1);
                                nonSymmetricTiles.Add(point2);
                            }
                            else
                            {
                                symmetricTiles.Add(point1);
                                symmetricTiles.Add(point2);
                            }
                        }
                    }
                }
            }
        }

        public float RatioThreshold;

        public SymmetricHorizontalStep(float ratioTiles = 0f) : base()
        {
            RatioThreshold = ratioTiles;
        }

        protected override void Init(StepInfo info)
        {
            info.AddInterest(new Interest());
        }

        protected override StepResult ProvideResult(StepInfo info)
        {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            return new StepResult("NotSymmetric")
            {
                success = interest.symmetricTiles.Count / (float)(interest.symmetricTiles.Count + interest.nonSymmetricTiles.Count) >= 1f - RatioThreshold,
                interest = interest.nonSymmetricTiles,
            };
        }
    }
}