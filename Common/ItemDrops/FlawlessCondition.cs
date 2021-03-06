using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class FlawlessCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.Aequus.DropCondition.Flawless");
        }
    }
}
