using Aequus.NPCs.Town.CarpenterNPC.Quest.Bounties;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Aequus.Content.Building.Passes.Steps {
    public class FurnitureCountStep : Step {
        public class Interest : StepInterest {
            public Dictionary<Point, List<Point>> givenHouses;

            public int decorAmt;
            public Dictionary<int, List<int>> tileStyleData;
            public List<Point> interestPoints;
            public List<Point> repeatPoints;

            public override void CompileInterestingPoints(StepInfo info) {
                decorAmt = 0;
                tileStyleData = new Dictionary<int, List<int>>();
                interestPoints = new List<Point>();
                repeatPoints = new List<Point>();

                foreach (var h in givenHouses.Values) {
                    foreach (var p in h) {
                        if (info[p].HasTile && !info[p].IsSolid) {
                            int style = Helper.GetTileStyle(info[p].TileType, info[p].TileFrameX, info[p].TileFrameY);
                            if (tileStyleData.TryGetValue(info[p].TileType, out List<int> compareStyle)) {
                                if (compareStyle.Contains(style)) {
                                    repeatPoints.Add(p);
                                    continue;
                                }
                                compareStyle.Add(style);
                            }
                            else {
                                tileStyleData.Add(info[p].TileType, new List<int>() { style });
                            }

                            interestPoints.Add(p);
                            decorAmt++;
                        }
                    }
                }
            }
        }

        public int MinimumFurniture;

        public FurnitureCountStep(int minFurniture) : base() {
            MinimumFurniture = minFurniture;
        }

        protected override void Init(StepInfo info) {
            info.AddInterest(new Interest());
        }

        protected override StepResult ProvideResult(StepInfo info) {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            return new StepResult("NotEnoughFurniture") {
                success = interest.decorAmt >= MinimumFurniture,
                interest = interest.interestPoints,
            };
        }

        public override string GetStepText(CarpenterBounty bounty) {
            return GetStepText(bounty, MinimumFurniture);
        }
    }
}