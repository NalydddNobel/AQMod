using Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;
using Aequus.Content.Enemies.PollutedOcean.OilSlime;
using Aequus.Content.Enemies.PollutedOcean.Scavenger;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanSystem : ModSystem {
    public static List<int> BiomeTiles { get; private set; } = new();

    public static int PollutedTileThreshold { get; set; } = 800;
    public static int PollutedTileMax { get; set; } = 300;
    public static int PollutedTileCount { get; set; }

    private static int? _music;
    public static int Music => _music ??= MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/PollutedOcean");

    public override void Unload() {
        BiomeTiles.Clear();
        _music = null;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        PollutedTileCount = 0;
        foreach (var tile in BiomeTiles) {
            PollutedTileCount += tileCounts[tile];
        }
    }

    public static bool CheckBiome(Player player) {
        return WorldGen.oceanDepths((int)player.position.X / 16, (int)Main.worldSurface) && PollutedTileCount >= PollutedTileMax;
    }
}