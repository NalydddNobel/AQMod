using Aequus.Common.NPCs;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules;

public class ConditionBossNoDamageTaken : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        if (info.npc == null || !info.npc.boss || !info.npc.TryGetGlobalNPC(out DamagedPlayersTracker noHitTracker)) { 
            return false; 
        }

        return !noHitTracker.anyInteractedPlayersAreDamaged;
    }

    public bool CanShowItemDropInUI() => false;
    public string GetConditionDescription() => null;
}
