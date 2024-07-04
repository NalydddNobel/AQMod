using AequusRemake.Core.Components;
using AequusRemake.DataSets;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AequusRemake.Content.Biomes.PollutedOcean.Generation;
internal class PollutedCleanupStep : AGenStep {
    public override string InsertAfter => "Stalac";

    protected override double GenWeight => 30f;

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        SetMessage(progress);
        ModContent.GetInstance<PollutedBiomeStep>().GetIterationValues(out int left, out int right, out int top, out int bottom);
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                SetProgress(progress, RectangleProgress(i, j, left, right, top, bottom));

                Tile tile = Main.tile[i, j];
                if (!TileDataSet.Polluted.Contains(tile.TileType)) {
                    continue;
                }

                for (int k = i - 1; k <= i + 1; k++) {
                    for (int l = j - 1; l <= j + 1; l++) {
                        if (TileDataSet.RemovedInPollutedOceanGen.Contains(Main.tile[k, l].TileType)) {
                            WorldGen.KillTile(k, l);
                        }
                    }
                }
            }
        }
    }
}
