using Aequus.Common.Building;
using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Content.Building.Passes {
    internal class CraftableTileStep : StepRequirement<ScanInfo, CraftableTileStep.Parameters> {
        public record struct Parameters(int RequiredTiles, Rectangle InputRectangle, ScanMap<bool> outputValidTiles, ScanMap<bool> outputInvalidTiles) : IStepRequirementParameters {
        }

        public override IStepResults GetStepResults(in ScanInfo info, in Parameters parameters = default(Parameters)) {
            int score = 0;
            var tempInvalidTiles = new ScanMap<bool>(parameters.outputInvalidTiles.Width, parameters.outputInvalidTiles.Height);
            for (int i = parameters.InputRectangle.X; i < parameters.InputRectangle.X + parameters.InputRectangle.Width; i++) {
                for (int j = parameters.InputRectangle.Y; j < parameters.InputRectangle.Y + parameters.InputRectangle.Height; j++) {
                    if (!info[i, j].IsFullySolid()) {
                        continue;
                    }

                    if (TileSets.IsTileIDCraftable(info[i, j].TileType)) {
                        score++;
                        parameters.outputValidTiles.SafeSet(i, j, true);
                    }
                    else {
                        tempInvalidTiles.SafeSet(i, j, true);
                        score--;
                    }
                }
            }
            if (score < parameters.RequiredTiles) {
                for (int i = 0; i < parameters.outputInvalidTiles.Width; i++) {
                    for (int j = 0; j < parameters.outputInvalidTiles.Height; j++) {
                        if (tempInvalidTiles[i, j]) {
                            parameters.outputInvalidTiles[i, j] = true;
                        }
                    }
                }
            }
            return new StepResultRatioPercent(score, parameters.RequiredTiles);
        }
    }
}
