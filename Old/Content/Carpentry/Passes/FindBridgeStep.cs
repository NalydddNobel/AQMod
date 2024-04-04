using Aequus.Common.Carpentry.Results;
using Aequus.Old.Common.Carpentry;
using Aequus.Old.Common.Carpentry.Results;
using Aequus.Old.Content.Building.Carpentry.Results;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;

namespace Aequus.Old.Content.Carpentry.Passes {
    public class FindBridgeStep : StepRequirement<ScanInfo, FindBridgeStep.Parameters> {
        public record struct Parameters(int RequiredMaxWaterDepth, int RequiredWaterAmount, int RequiredLiquidType, int BridgeLengthWanted, Rectangle InputRectangle, ScanMap<bool> outputValidTiles, ScanMap<bool> outputInvalidTiles) : IStepRequirementParameters {
            public StrongBox<Rectangle> BridgeLocation = new();
        }

        public override IScanResults GetStepResults(in ScanInfo info, in Parameters parameters = default(Parameters)) {
            var bounds = parameters.InputRectangle;
            int waterX = 0;
            int waterY = 0;
            int liquidIDCache = 0;
            int waterDepthCache = 0;
            int waterTileCountCache = 0;
            for (int i = 0; i < parameters.InputRectangle.Width; i++) {
                for (int j = 0; j < parameters.InputRectangle.Height; j++) {
                    int x = i + parameters.InputRectangle.X;
                    int y = j + parameters.InputRectangle.Y;
                    var tile = Framing.GetTileSafely(x, y);
                    if (tile.LiquidAmount > 0 && !tile.IsFullySolid()) {
                        int bridgeLength = 0;
                        liquidIDCache = tile.LiquidType;
                        for (int k = i; k < parameters.InputRectangle.Width; k++) {
                            int waterLineX = k + parameters.InputRectangle.X;
                            var waterLineTile = Framing.GetTileSafely(waterLineX, y);
                            bool foundBridgeTile = false;
                            for (int l = j; l > 0; l--) {
                                if (Framing.GetTileSafely(waterLineX, l + parameters.InputRectangle.Y).IsFullySolid()) {
                                    for (int n = l - 1; n > 0 && n > l - 3; n--) {
                                        if (Framing.GetTileSafely(waterLineX, n + parameters.InputRectangle.Y).IsFullySolid()) {
                                            goto InnerLoopContinueLabel;
                                        }
                                    }
                                    foundBridgeTile = true;
                                    break;

                                InnerLoopContinueLabel:
                                    if (bridgeLength == 0) {
                                        goto ContinueFindingBridgesLabel;
                                    }
                                    continue;
                                }
                            }

                            if (bridgeLength == 0) {
                                if (!foundBridgeTile) {
                                    goto ContinueFindingBridgesLabel;
                                }
                            }
                            else if (waterLineTile.LiquidAmount <= 0 || waterLineTile.LiquidType != liquidIDCache || waterLineTile.IsFullySolid() || k >= parameters.InputRectangle.Width - 1 || !foundBridgeTile) {
                                waterX = i;
                                waterY = j;
                                int bridgeY = Math.Max(j - 12 + parameters.InputRectangle.Y, 0);
                                parameters.BridgeLocation.Value = new(x, bridgeY, k - waterX, 13);
                                if (k == parameters.InputRectangle.Width - 1) {
                                    parameters.BridgeLocation.Value.Width++;
                                }
                                goto CheckWater;
                            }
                            bridgeLength++;
                        }

                    ContinueFindingBridgesLabel:
                        continue;
                    }
                }
            }

            if (waterX == 0 || waterY == 0) {
                return new StepResultRatio(0, parameters.BridgeLengthWanted);
            }

        CheckWater:
            var list = new List<Point>() { new Point(waterX + parameters.InputRectangle.X, waterY + parameters.InputRectangle.Y), };
            var alreadyChecked = new List<Point>();
            while (list.Count <= 1000 && list.Count > 0) {
                var p = list[0];
                list.RemoveAt(0);
                if (!WorldGen.InWorld(p.X, p.Y) || !bounds.Contains(p)) {
                    alreadyChecked.Add(p);
                    continue;
                }
                var t = Main.tile[p];
                if (!t.IsFullySolid() && t.LiquidAmount > 0 && t.LiquidType == liquidIDCache && !alreadyChecked.Contains(p)) {
                    waterTileCountCache++;
                    waterDepthCache = Math.Max(p.Y - waterY, waterDepthCache);
                    if (p.X + 1 < parameters.BridgeLocation.Value.X + parameters.BridgeLocation.Value.Width) {
                        list.Add(new(p.X + 1, p.Y));
                    }
                    if (p.X - 1 > parameters.BridgeLocation.Value.X) {
                        list.Add(new(p.X - 1, p.Y));
                    }
                    list.Add(new(p.X, p.Y + 1));
                    list.Add(new(p.X, p.Y - 1));
                }
                alreadyChecked.Add(p);
            }

            var output = waterTileCountCache < parameters.RequiredWaterAmount || liquidIDCache != parameters.RequiredLiquidType || waterDepthCache < parameters.RequiredMaxWaterDepth || parameters.BridgeLocation.Value.Width < parameters.BridgeLengthWanted ? parameters.outputInvalidTiles : parameters.outputValidTiles;
            foreach (var p in alreadyChecked) {
                var t = Framing.GetTileSafely(p);
                if (!t.IsFullySolid() && t.LiquidAmount > 0 && t.LiquidType == liquidIDCache) {
                    output.SafeSet(p.X - bounds.X, p.Y - bounds.Y, true);
                }
            }
            return new StepResultRatio(parameters.BridgeLocation.Value.Width, parameters.BridgeLengthWanted);
        }
    }
}
