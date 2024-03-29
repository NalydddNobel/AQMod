﻿using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules {
    internal class HardmodeTierCondition : IItemDropRuleCondition {
        public bool CanDrop(DropAttemptInfo info) {
            return Aequus.HardmodeTier;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI() {
            return Aequus.HardmodeTier;
        }

        public string GetConditionDescription() {
            return null;
        }
    }
}