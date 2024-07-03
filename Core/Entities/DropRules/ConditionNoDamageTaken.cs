using Aequu2.Core.Entities.NPCs;
using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Core.Entities.Items.DropRules;

public class ConditionNoDamageTaken : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        if (info.npc == null || !info.npc.TryGetGlobalNPC(out DamagedPlayersTracker noHitTracker)) {
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
