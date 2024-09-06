using Aequus.Common.Preferences;
using Aequus.Common.Utilities.Helpers;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Villagers;

public class NPCSettleDownMessage : ModSystem {
    public static NPCSettleDownMessage Instance => ModContent.GetInstance<NPCSettleDownMessage>();

    int _lastNPCAnnounced;
    bool _anyWantedNPCsMovedInToday;

    readonly HashSet<int> Blacklist = [NPCID.Truffle, NPCID.SantaClaus];

    [Gen.AequusNPC_OnSpawn]
    public static void OnSpawn(NPC npc, IEntitySource source) {
        if (npc.townNPC && WorldGen.prioritizedTownNPCType > 0 && WorldGen.prioritizedTownNPCType == npc.type) {
            // This disables the message temporarily as a villager has successfully moved in!
            Instance._anyWantedNPCsMovedInToday = true;
        }
    }

    public override void Load() {
        On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;
    }

    #region Hooks
    void TryAnnounce() {
        // If a villager moved in yesterday, or an invasion is occuring, do not display a message.
        if (_anyWantedNPCsMovedInToday || NPC.BusyWithAnyInvasionOfSorts()) {
            return;
        }

        int wantedNPC = 0;
        int maxNPCs = Math.Min(Main.townNPCCanSpawn.Length, NPCLoader.NPCCount);
        for (int i = 0; i < maxNPCs; i++) {
            if (Main.townNPCCanSpawn[i] && !NPCID.Sets.IsTownPet[i] && !NPC.AnyNPCs(wantedNPC)) {
                wantedNPC = i;
                break;
            }
        }

        if (wantedNPC == 0 || wantedNPC == _lastNPCAnnounced) {
            return;
        }

        LocalizedText npcName = Lang.GetNPCName(wantedNPC);
        WorldGen.BroadcastText(NetworkText.FromKey("Mods.Aequus.Announcement.NPCWantsToMoveIn", npcName), CommonColor.TextVillagerHasArrived);
        _lastNPCAnnounced = wantedNPC;
    }

    void OnStartDay() {
        TryAnnounce();
        _anyWantedNPCsMovedInToday = false;
    }

    private static void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents) {
        orig(ref stopEvents);
        Instance.OnStartDay();
    }
    #endregion

    #region Config
    // Prevent this system from loading depending on the config.
    public override bool IsLoadingEnabled(Mod mod) {
        return GameplayConfig.Instance.ShowNPCSettleDownMessage;
    }
    #endregion

    #region Mod Calls
    [ModCall("BlacklistSettleDownMsg")]
    public static bool SetBlacklist(int npcId, bool value) {
        if (value) {
            return Instance.Blacklist.Add(npcId);
        }

        return Instance.Blacklist.Remove(npcId);
    }

    [ModCall(Name: "NPCSettleDownMsgContains")]
    public static bool InBlacklist(int npcId) {
        return Instance.Blacklist.Contains(npcId);
    }
    #endregion
}
