using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops
{
    public class HardmodeTierCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return Aequus.HardmodeTier;
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
