using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops
{
    public class OreTierCondition : IItemDropRuleCondition
    {
        private int tier;
        private int textType;
        public bool alt;

        public OreTierCondition(int tier, bool isAlt)
        {
            this.tier = tier;
            alt = isAlt;
            textType = tier * 2 + (alt ? 1 : 0);
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return CanDropOrShow();
        }

        public bool CanShowItemDropInUI()
        {
            return CanDropOrShow();
        }
        public bool CanDropOrShow()
        {
            return Main.drunkWorld || AequusSystem.OreTiers()[tier] == !alt;
        }

        public string GetConditionDescription()
        {
            return AequusText.GetText("DropCondition.OreTier." + textType);
        }
    }
}