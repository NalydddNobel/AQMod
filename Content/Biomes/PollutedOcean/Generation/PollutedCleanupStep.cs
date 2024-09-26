#if POLLUTED_OCEAN
using Aequus.Systems.WorldGeneration;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.PollutedOcean.Generation;
internal class PollutedCleanupStep : AGenStep {
    public override string InsertAfter => "Stalac";

    protected override double Weight => 30f;

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        SetMessage(progress);
        var isPollutedTile = ModContent.GetInstance<PollutedOceanSystem>().IsPolluted;
        var removeFromGen = ModContent.GetInstance<PollutedOceanSystem>().RemoveFromGen;
        ModContent.GetInstance<PollutedBiomeStep>().GetIterationValues(out int left, out int right, out int top, out int bottom);
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                SetProgress(progress, RectangleProgress(i, j, left, right, top, bottom));

                Tile tile = Main.tile[i, j];
                if (!isPollutedTile.Contains(tile.TileType)) {
                    continue;
                }

                for (int k = i - 1; k <= i + 1; k++) {
                    for (int l = j - 1; l <= j + 1; l++) {
                        if (removeFromGen.Contains(Main.tile[k, l].TileType)) {
                            WorldGen.KillTile(k, l);
                        }
                    }
                }
            }
        }
    }
}
#endif