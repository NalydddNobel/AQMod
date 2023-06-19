using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDropRules {
    public class IsHalloweenCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return Main.halloween;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI()
        {
            return Main.halloween;
        }

        public string GetConditionDescription()
        {
            return null;
        }
    }
}