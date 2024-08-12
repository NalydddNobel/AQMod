using System;

namespace Aequus.Content.Biomes.Oblivion.Tiles;

public class OblivionAltarCount : ModSystem {
    public static int TileCount { get; private set; }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        TileCount = tileCounts[ModContent.TileType<OblivionAltar>()];
    }
}
