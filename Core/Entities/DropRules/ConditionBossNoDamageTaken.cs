using AequusRemake.Core.Entities.NPCs;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Core.Entities.Items.DropRules;

public class ConditionBossNoDamageTaken : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        if (info.npc == null || !info.npc.boss || !info.npc.TryGetGlobalNPC(out DamagedPlayersTracker noHitTracker)) {
            return false;
        }

        return !noHitTracker.anyInteractedPlayersAreDamaged;
    }

    public bool CanShowItemDropInUI() {
        return false;
    }

    public string GetConditionDescription() {
        return null;
    }
}
