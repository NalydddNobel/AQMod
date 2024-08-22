using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules {
    internal class HardmodeTierCondition : IItemDropRuleCondition {
        public bool CanDrop(DropAttemptInfo info) {
            return Aequus.MediumMode;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI() {
            return Aequus.MediumMode;
        }

        public string GetConditionDescription() {
            return null;
        }
    }
}