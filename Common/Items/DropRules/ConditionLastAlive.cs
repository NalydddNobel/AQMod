using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules;

public class ConditionLastAlive : IItemDropRuleCondition, IProvideItemConditionDescription {
    private readonly System.Int32 type;

    public ConditionLastAlive(System.Int32 type) {
        this.type = type;
    }

    public System.Boolean CanDrop(DropAttemptInfo info) {
        return NPC.CountNPCS(type) <= 1;
    }

    public System.Boolean CanShowItemDropInUI() {
        return true;
    }

    public System.String GetConditionDescription() {
        return null;
    }
}