using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Core.Entities.Items.DropRules;

public class ConditionLastAlive : IItemDropRuleCondition, IProvideItemConditionDescription {
    private readonly int type;

    public ConditionLastAlive(int type) {
        this.type = type;
    }

    public bool CanDrop(DropAttemptInfo info) {
        return NPC.CountNPCS(type) <= 1;
    }

    public bool CanShowItemDropInUI() {
        return true;
    }

    public string GetConditionDescription() {
        return null;
    }
}