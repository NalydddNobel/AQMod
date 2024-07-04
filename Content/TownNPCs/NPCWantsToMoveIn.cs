using System;
using Terraria.DataStructures;
using Terraria.Localization;

namespace AequusRemake.Content.TownNPCs;

public class NPCWantsToMoveIn : GlobalNPC {
    private static int _lastNPCAnnounced;
    private static bool _anyWantedNPCsMovedInToday;

    public static void OnStartDay() {
        TryAnnounce();
        _anyWantedNPCsMovedInToday = false;
    }

    public static void TryAnnounce() {
        if (_anyWantedNPCsMovedInToday || NPC.BusyWithAnyInvasionOfSorts()) {
            return;
        }

        int wantedNPC = 0;
        int maxNPCs = Math.Min(Main.townNPCCanSpawn.Length, NPCLoader.NPCCount);
        for (int i = 0; i < maxNPCs; i++) {
            if (Main.townNPCCanSpawn[i] && !NPCSets.IsTownPet[i] && !NPC.AnyNPCs(wantedNPC)) {
                wantedNPC = i;
                break;
            }
        }

        if (wantedNPC == 0 || wantedNPC == _lastNPCAnnounced) {
            return;
        }

        LocalizedText npcName = Lang.GetNPCName(wantedNPC);
        WorldGen.BroadcastText(NetworkText.FromKey("Mods.AequusRemake.Announcement.NPCWantsToMoveIn", npcName), CommonColor.TextVillagerHasArrived);
        _lastNPCAnnounced = wantedNPC;
    }

    public override void OnSpawn(NPC npc, IEntitySource source) {
        if (npc.townNPC && WorldGen.prioritizedTownNPCType > 0 && WorldGen.prioritizedTownNPCType == npc.type) {
            _anyWantedNPCsMovedInToday = true;
        }
    }
}
