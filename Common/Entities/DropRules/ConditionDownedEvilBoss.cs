using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.Entities.DropRules;

public class ConditionDownedEvilBoss : IItemDropRuleCondition {
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) {
        return NPC.downedBoss2;
    }

    bool IItemDropRuleCondition.CanShowItemDropInUI() {
        return NPC.downedBoss2;
    }

    string IProvideItemConditionDescription.GetConditionDescription() {
        return Language.GetTextValue("Conditions.DownedBoss2");
    }
}
