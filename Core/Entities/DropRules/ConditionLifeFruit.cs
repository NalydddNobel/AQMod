using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Core.Entities.Items.DropRules;

public class ConditionLifeFruit : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        return info.player.ConsumedLifeFruit < Player.LifeFruitMax;
    }

    public bool CanShowItemDropInUI() {
        return true;
    }

    public string GetConditionDescription() {
        return null;
    }
}
