using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;

namespace Aequus.Core.Utilities;

public static class NPCHelper {
    public static bool TryRetargeting(this NPC npc, bool faceTarget = true) {
        npc.TargetClosest(faceTarget: faceTarget);
        return npc.HasValidTarget;
    }

    private static bool BuffImmuneCommon(int npcId, out NPCDebuffImmunityData buffImmunities) {
        if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(npcId, out buffImmunities) || buffImmunities == null || buffImmunities.ImmuneToAllBuffsThatAreNotWhips) {
            return true;
        }

        return false;
    }

    #region Buffs
    public static void SetBuffImmune(int npcID, int buffID) {
        if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(npcID, out var data)) {
            NPCID.Sets.DebuffImmunitySets[npcID] = new() { SpecificallyImmuneTo = new[] { buffID } };
            return;
        }

        if (data.ImmuneToAllBuffsThatAreNotWhips || data.SpecificallyImmuneTo.Contains(buffID)) {
            return;
        }

        Array.Resize(ref data.SpecificallyImmuneTo, data.SpecificallyImmuneTo.Length + 1);
        data.SpecificallyImmuneTo[^1] = buffID;
    }

    public static bool IsBuffsImmune(int npcId, params int[] buffIds) {
        return !BuffImmuneCommon(npcId, out var buffImmunities) && buffImmunities.SpecificallyImmuneTo != null && buffImmunities.SpecificallyImmuneTo.ContainsAny(buffIds);
    }

    public static bool IsBuffImmune(int npcId, int buffId) {
        return !BuffImmuneCommon(npcId, out var buffImmunities) && buffImmunities.SpecificallyImmuneTo != null && buffImmunities.SpecificallyImmuneTo.ContainsAny(buffId);
    }
    #endregion
}
