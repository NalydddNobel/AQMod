using Aequus.Common.DataSets;
using Aequus.Content.Building.old.Quest.Bounties;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Building.old.Steps {
    public class CraftableTilesStep : Step {
        public class Interest : StepInterest {
            public List<Point> craftableTiles;
            public List<Point> nonCraftableTiles;
            public Rectangle resultRectangle;

            public Rectangle givenRectangle;

            public override void CompileInterestingPoints(StepInfo info) {
                nonCraftableTiles = new List<Point>();
                craftableTiles = new List<Point>();
                for (int i = givenRectangle.X; i < givenRectangle.X + givenRectangle.Width; i++) {
                    for (int j = givenRectangle.Y; j < givenRectangle.Y + givenRectangle.Height; j++) {
                        if (info.Map.InSceneRenderedMap(i, j) && info[i, j].IsFullySolid) {
                            if (TileSets.IsTileIDCraftable(info[i, j].TileType)) {
                                craftableTiles.Add(new Point(i, j));
                            }
                            else {
                                nonCraftableTiles.Add(new Point(i, j));
                            }
                        }
                    }
                }
                TotalSurroundings(info);
                Trim();
            }

            public void TotalSurroundings(StepInfo info) {
                resultRectangle = new Rectangle(info.Width, info.Height, 1, 1);
                foreach (var p in craftableTiles) {
                    resultRectangle.X = Math.Min(resultRectangle.X, p.X);
                    resultRectangle.Y = Math.Min(resultRectangle.Y, p.Y);
                }
                foreach (var p in craftableTiles) {
                    resultRectangle.Width = Math.Max(p.X - resultRectangle.X, resultRectangle.Width);
                    resultRectangle.Height = Math.Max(p.Y - resultRectangle.Y, resultRectangle.Height);
                }
            }

            public void Trim() {
                var remove = new List<Point>();
                foreach (var p in nonCraftableTiles) {
                    if (!resultRectangle.Contains(p)) {
                        remove.Add(p);
                    }
                }
                foreach (var p in remove) {
                    nonCraftableTiles.Remove(p);
                }
            }
        }

        public int MinimumTiles;
        public float RatioThreshold;

        public CraftableTilesStep(int minTiles = 12, float ratioTiles = 0f) : base() {
            MinimumTiles = minTiles;
            RatioThreshold = ratioTiles;
        }

        protected override void Init(StepInfo info) {
            info.AddInterest(new Interest());
        }

        protected override StepResult ProvideResult(StepInfo info) {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            return new StepResult("NoCraftedBlocks") {
                success = interest.craftableTiles.Count >= MinimumTiles
                && interest.craftableTiles.Count / (float)(interest.craftableTiles.Count + interest.nonCraftableTiles.Count) >= 1f - RatioThreshold,
                interest = interest.nonCraftableTiles,
            };
        }

        public override string GetStepText(CarpenterBounty bounty) {
            if (RatioThreshold > 0f)
                return Language.GetTextValue("Mods.Aequus.CarpenterBounty.Rule.CraftableTilesStep_2", $"{Math.Floor((1f - RatioThreshold) * 100f)}%");
            return base.GetStepText(bounty);
        }
    }
}