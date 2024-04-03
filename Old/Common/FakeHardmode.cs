using Aequus.Core;
using System.Collections.Generic;

namespace Aequus.Old.Common;

public class FakeHardmode {
    public static void AddEnemies(IDictionary<int, float> pool, in NPCSpawnInfo spawnInfo) {
        if (Main.hardMode || !WorldState.HardmodeTier) {
            return;
        }

        if (spawnInfo.Player.ZoneMarble) {
            pool[NPCID.Medusa] = 0.1f;
        }
        if (spawnInfo.Player.ZoneRockLayerHeight) {
            if (!NPC.savedWizard && !NPC.AnyNPCs(NPCID.Wizard)) {
                pool[NPCID.BoundWizard] = 0.01f;
            }
        }
        if (spawnInfo.Player.ZoneUnderworldHeight) {
            if (!NPC.savedTaxCollector && !NPC.AnyNPCs(NPCID.DemonTaxCollector)) {
                pool[NPCID.DemonTaxCollector] = 0.05f;
            }
        }
    }
}
