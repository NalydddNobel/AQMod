using Aequus.Common.Preferences;
using System.Collections.Generic;

namespace Aequus.Content.Monsters.Hallow;

public class HallowEnemiesGlobalNPC : GlobalNPC {
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (!GameplayConfig.Instance.EarlyMimics || Main.hardMode || !spawnInfo.Player.ZoneHallow || spawnInfo.SpawnTileY > ((int)Main.worldSurface) || !spawnInfo.Allowed()) {
            return;
        }

        if (!Main.dayTime) {
            pool[NPCID.Gastropod] = 0.1f;
        }
        //if (Main.tileSand[Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType]) {
        //    pool[NPCID.LightMummy] = 0.1f;
        //}
        pool[NPCID.Pixie] = 0.1f;
    }
}
