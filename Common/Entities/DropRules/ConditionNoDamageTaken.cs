﻿using Aequus.Common.NPCs.Global;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Entities.Items.DropRules;

public class ConditionNoDamageTaken : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        if (info.npc == null || !info.npc.TryGetGlobalNPC(out FlawlessGlobalNPC noHitTracker)) {
            return false;
        }

        return noHitTracker.anyInteractedPlayersAreDamaged;
    }

    public bool CanShowItemDropInUI() {
        return false;
    }

    public string GetConditionDescription() {
        return null;
    }
}
