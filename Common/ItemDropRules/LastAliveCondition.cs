using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDropRules {
    public class LastAliveCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        private readonly int type;

        public LastAliveCondition(int type)
        {
            this.type = type;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return NPC.CountNPCS(type) <= 1;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return null;
        }
    }
}