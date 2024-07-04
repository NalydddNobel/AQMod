using AequusRemake.Content.Biomes.PollutedOcean;
using System.Collections.Generic;

namespace AequusRemake.Core.Entities.NPCs;
public class NPCSpawns : GlobalNPC {
    /// <summary>A list of <see cref="ModBiome"/>s which will override AequusRemake biome spawns.</summary>
    public readonly static List<ModBiome> OverrideAequusRemakeBiomes = new();

    public override void Unload() {
        OverrideAequusRemakeBiomes.Clear();
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
        if (player.InModBiome<PollutedOceanBiomeSurface>() || player.InModBiome<PollutedOceanBiomeUnderground>()) {
            maxSpawns = (int)(maxSpawns * 1.5f);
        }
    }

    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerSolar || spawnInfo.Player.ZoneTowerStardust || spawnInfo.Player.ZoneTowerVortex || spawnInfo.Lihzahrd || Main.wallDungeon[Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].WallType]) {
            return;
        }

        if (spawnInfo.Invasion) {
            return;
        }

        foreach (var b in OverrideAequusRemakeBiomes) {
            if (spawnInfo.Player.InModBiome(b)) {
                return;
            }
        }

        if (spawnInfo.Player.InModBiome<PollutedOceanBiomeSurface>()) {
            PollutedOceanSystem.PopulateSurfaceSpawnPool(pool, spawnInfo);
        }
        else if (spawnInfo.Player.InModBiome<PollutedOceanBiomeUnderground>()) {
            PollutedOceanSystem.PopulateUndergroundSpawnPool(pool, spawnInfo);
        }
    }
}
