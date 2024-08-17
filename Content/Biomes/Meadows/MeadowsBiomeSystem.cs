using Aequus.Content.Biomes.Meadows.Tiles;
using System;

namespace Aequus.Content.Biomes.Meadows;

public class MeadowsBiomeSystem : ModSystem {
    public int TileCount { get; private set; }
    public static readonly int TileCountNeeded = 20;

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        TileCount = tileCounts[ModContent.TileType<MeadowGrass>()];
    }
}
