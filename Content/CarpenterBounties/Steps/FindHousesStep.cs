using Aequus.Graphics.RenderTargets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.CarpenterBounties.Steps
{
    public class FindHousesStep : Step
    {
        public class Interest : StepInterest
        {
            public Dictionary<Point, List<Point>> housingWalls;
            public override void CompileInterestingPoints(StepInfo info)
            {
                int padding = ShutterstockerSceneRenderer.TilePaddingForChecking / 2;
                housingWalls = new Dictionary<Point, List<Point>>();
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
            }
        }

        public int MinimumHouses;

        public FindHousesStep(int minHouses) : base()
        {
            MinimumHouses = minHouses;
        }

        protected override void Init(StepInfo info)
        {
            info.AddInterest(new Interest());
        }

        protected override StepResult ProvideResult(StepInfo info)
        {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            return new StepResult("NoHouses")
            {
                success = interest.housingWalls.Count >= MinimumHouses,
            };
        }

        public override string GetStepKey(CarpenterBounty bounty)
        {
            return GetStepKey(bounty, MinimumHouses);
        }
    }
}