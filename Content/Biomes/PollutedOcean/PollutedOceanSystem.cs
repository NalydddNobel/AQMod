using System;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanSystem : ModSystem {
    public static readonly int TileCountPriorityThreshold = 1000;
    public static readonly int TileCountBiome = 300;

    public HashSet<int> IsPolluted = [];
    public HashSet<int> RemoveFromGen = [TileID.Pots, TileID.Coral];

    public int TileCount { get; protected set; }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        TileCount = 0;
        foreach (int i in IsPolluted) {
            TileCount += tileCounts[i];
        }
    }
}
