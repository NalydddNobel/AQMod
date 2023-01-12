using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Carpentery.Bounties.Steps
{
    public class FindBridgeStep : Step
    {
        public class Interest : StepInterest
        {
            public Rectangle bridgeLocation;
            public int waterX;
            public int waterY;
            public int waterDepthCache;
            public int waterTileCountCache;
            public int liquidIDCache;

            public override void CompileInterestingPoints(StepInfo info)
            {
                bridgeLocation = new Rectangle();
                waterX = 0;
                waterY = 0;
                liquidIDCache = 0;
                waterDepthCache = 0;
                waterTileCountCache = 0;
                int padding = PhotoRenderer.TilePaddingForChecking / 2;
                for (int i = padding; i < info.Width - padding; i++)
                {
                    for (int j = padding; j < info.Height - padding; j++)
                    {
                        if (info[i, j].LiquidAmount > 0 && !info[i, j].IsFullySolid)
                        {
                            liquidIDCache = info[i, j].LiquidType;
                            for (int l = j; l > padding; l--)
                            {
                                if (info[i, l].IsFullySolid)
                                {
                                    for (int k = i; k < info.Width - padding; k++)
                                    {
                                        if (info[k, j].LiquidAmount <= 0 || info[k, j].LiquidType != liquidIDCache || info[k, j].IsFullySolid || k >= info.Width - padding - 1)
                                        {
                                            waterX = i;
                                            waterY = j;
                                            bridgeLocation.X = i;
                                            bridgeLocation.Y = Math.Max(l - 8, padding);
                                            bridgeLocation.Width = k - waterX;
                                            bridgeLocation.Height = waterY - bridgeLocation.Y;
                                            goto CheckWater;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (waterX == 0 || waterY == 0)
                    return;

                CheckWater:
                var list = new List<Point>() { new Point(waterX, waterY), };
                var alreadyChecked = new List<Point>();
                while (waterTileCountCache <= 1000 && list.Count > 0)
                {
                    var p = list[0];
                    if (!info.Map.InSceneRenderedMap(list[0]))
                    {
                        alreadyChecked.Add(list[0]);
                        list.RemoveAt(0);
                        continue;
                    }
                    var t = info[p];
                    if (!t.IsFullySolid && t.LiquidAmount > 0 && t.LiquidType == liquidIDCache && !alreadyChecked.Contains(p))
                    {
                        waterTileCountCache++;
                        waterDepthCache = Math.Max(p.Y - waterY, waterDepthCache);
                        list.Add(new Point(p.X + 1, p.Y));
                        list.Add(new Point(p.X - 1, p.Y));
                        list.Add(new Point(p.X, p.Y + 1));
                        list.Add(new Point(p.X, p.Y - 1));
                    }
                    alreadyChecked.Add(list[0]);
                    list.RemoveAt(0);
                }
            }
        }

        public int WaterTilesNeeded;
        public int WaterHeightNeeded;
        public int LiquidIDWanted;
        public int BridgeLengthWanted;

        public FindBridgeStep(int waterTilesNeeded, int waterHeightNeeded, int liquidIDWanted, int bridgeLengthWanted) : base()
        {
            WaterTilesNeeded = waterTilesNeeded;
            WaterHeightNeeded = waterHeightNeeded;
            LiquidIDWanted = liquidIDWanted;
            BridgeLengthWanted = bridgeLengthWanted;
        }

        protected override void Init(StepInfo info)
        {
            info.AddInterest(new Interest());
        }

        protected override StepResult ProvideResult(StepInfo info)
        {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            //Terraria.Main.NewText(interest.waterX);
            //Terraria.Main.NewText(interest.waterY);
            //Terraria.Main.NewText(interest.bridgeLocation.X);
            //Terraria.Main.NewText(interest.bridgeLocation.Y);
            //Terraria.Main.NewText(interest.bridgeLocation.Width);
            //Terraria.Main.NewText(interest.bridgeLocation.Height, Color.Yellow);
            return new StepResult("NoBridge")
            {
                success = interest.waterTileCountCache > WaterTilesNeeded && interest.waterDepthCache > WaterHeightNeeded
                && interest.liquidIDCache == LiquidIDWanted && interest.bridgeLocation.Width > BridgeLengthWanted,
                interest = new List<Point>()
                {
                    new Point(interest.waterX, interest.waterY),
                    new Point(interest.bridgeLocation.X, interest.bridgeLocation.Y),
                    new Point(interest.bridgeLocation.X + interest.bridgeLocation.Width, interest.bridgeLocation.Y),
                    new Point(interest.bridgeLocation.X, interest.bridgeLocation.Y + interest.bridgeLocation.Height),
                    new Point(interest.bridgeLocation.X + interest.bridgeLocation.Width, interest.bridgeLocation.Y + interest.bridgeLocation.Height),
                }
            };
        }

        public override string GetStepText(CarpenterBounty bounty)
        {
            return GetStepText(bounty, BridgeLengthWanted, WaterHeightNeeded);
        }
    }
}
