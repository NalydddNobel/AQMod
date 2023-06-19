using System;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDropRules {
    public class OnFirstKillCondition : IItemDropRuleCondition {
        public Func<bool> DefeatedFlag;

        public OnFirstKillCondition(Func<bool> wasDefeated) {

        }

        public bool CanDrop(DropAttemptInfo info) {
            return DefeatedFlag();
        }

        public bool CanShowItemDropInUI() {
            return false;
        }

        public string GetConditionDescription() {
            return null;
        }
    }
}
