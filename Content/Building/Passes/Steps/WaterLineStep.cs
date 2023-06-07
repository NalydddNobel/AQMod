using Aequus.NPCs.Town.CarpenterNPC.Quest.Bounties;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Aequus.Content.Building.Passes.Steps {
    public class WaterLineStep : Step {
        public class Interest : StepInterest {
            public Dictionary<Point, List<Point>> givenHouses;

            public int waterLine;

            public override void CompileInterestingPoints(StepInfo info) {
                int waterY = 1000;
                foreach (var h in givenHouses.Values) {
                    foreach (var p in h) {
                        if (p.Y < waterY) {
                            for (int i = 0; i < 1000; i++) {
                                var checkPoint = new Point(p.X + i, p.Y);
                                if (!info.Map.InSceneRenderedMap(checkPoint) || checkPoint.Y >= waterY) {
                                    break;
                                }
                                if (info[checkPoint].LiquidAmount > 0) {
                                    waterY = checkPoint.Y;
                                    break;
                                }
                                if (!info[checkPoint].IsFullySolid && !h.Contains(checkPoint)) {
                                    break;
                                }
                            }

                            for (int i = 0; i < 1000; i++) {
                                var checkPoint = new Point(p.X - i, p.Y);
                                if (!info.Map.InSceneRenderedMap(checkPoint) || checkPoint.Y >= waterY) {
                                    break;
                                }
                                if (info[checkPoint].LiquidAmount > 0) {
                                    waterY = checkPoint.Y;
                                    break;
                                }
                                if (!info[checkPoint].IsFullySolid && !h.Contains(checkPoint)) {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (waterY == 1000) {
                    waterLine = 0;
                    return;
                }
                var cap = CarpenterBounty.GetSurroundings(info, givenHouses.Values);
                waterLine = cap.Y + cap.Height - waterY + 1;
            }
        }

        public int MinimumWaterLine;

        public WaterLineStep(int minWaterLine) : base() {
            MinimumWaterLine = minWaterLine;
        }

        protected override void Init(StepInfo info) {
            info.AddInterest(new Interest());
        }

        protected override StepResult ProvideResult(StepInfo info) {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            return new StepResult("WallsDontHaveEnoughWater") {
                success = interest.waterLine >= MinimumWaterLine,
            };
        }

        public override string GetStepText(CarpenterBounty bounty) {
            return GetStepText(bounty, MinimumWaterLine);
        }
    }
}