using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops
{
    internal class HardmodeTierCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return WorldFlags.HardmodeTier;
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
