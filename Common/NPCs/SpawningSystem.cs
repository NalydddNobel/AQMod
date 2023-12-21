using Aequus.Content.Biomes.PollutedOcean;
using System.Collections.Generic;

namespace Aequus.Common.NPCs;
public class SpawningSystem : GlobalNPC {
    /// <summary>
    /// A list of <see cref="ModBiome"/>s which will override Aequus biome spawns.
    /// </summary>
    public readonly static List<ModBiome> OverrideAequusBiomes = new();

    public override void Unload() {
        OverrideAequusBiomes.Clear();
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

        if (spawnInfo.Player.InModBiome<PollutedOceanBiome>()) {
            PollutedOceanBiome.PopulateSpawnPool(pool, spawnInfo);
            return;
        }
    }
}
