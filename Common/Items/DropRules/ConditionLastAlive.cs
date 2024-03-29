using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules;

public class ConditionLastAlive : IItemDropRuleCondition, IProvideItemConditionDescription {
    private readonly int type;

    public ConditionLastAlive(int type) {
        this.type = type;
    }

    public bool CanDrop(DropAttemptInfo info) => NPC.CountNPCS(type) <= 1;
    public bool CanShowItemDropInUI() => true;
    public string GetConditionDescription() => null;
}