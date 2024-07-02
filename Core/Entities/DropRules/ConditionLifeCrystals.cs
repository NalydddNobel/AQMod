using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Entities.Items.DropRules;

public class ConditionLifeCrystals : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        return info.player.ConsumedLifeCrystals < Player.LifeCrystalMax;
    }

    public bool CanShowItemDropInUI() {
        return true;
    }

    public string GetConditionDescription() {
        return null;
    }
}
