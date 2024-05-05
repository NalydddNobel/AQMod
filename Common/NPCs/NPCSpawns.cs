using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Old.Common;
using Aequus.Old.Common.NPCs;
using System.Collections.Generic;

namespace Aequus.Common.NPCs;
public class NPCSpawns : GlobalNPC {
    /// <summary>A list of <see cref="ModBiome"/>s which will override Aequus biome spawns.</summary>
    public readonly static List<ModBiome> OverrideAequusBiomes = new();

    public override void Unload() {
        OverrideAequusBiomes.Clear();
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

        foreach (var b in OverrideAequusBiomes) {
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

#if !DEBUG
        MimicEdits.AddPHMMimics(pool, in spawnInfo);
        FakeHardmode.AddEnemies(pool, in spawnInfo);
#endif
    }
}
