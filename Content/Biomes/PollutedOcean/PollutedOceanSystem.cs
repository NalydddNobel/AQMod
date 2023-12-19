using System;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanSystem : ModSystem {
    public static List<int> BiomeTiles { get; private set; } = new();

    public override void Unload() {
        BiomeTiles.Clear();
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        foreach (var tile in BiomeTiles) {
            PollutedOceanBiome.BlockPresence += tileCounts[tile];
        }
    }
}