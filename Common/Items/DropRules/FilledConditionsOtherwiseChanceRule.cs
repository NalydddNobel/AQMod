using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules {
    public class FilledConditionsOtherwiseChanceRule : IItemDropRule, IItemDropRuleCondition {
        public int ItemType;
        public int Chance;
        public IItemDropRuleCondition Condition;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public FilledConditionsOtherwiseChanceRule(IItemDropRuleCondition condition, int item, int chance) {
            ItemType = item;
            Chance = chance;
            Condition = condition;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
            float num = 1f / Chance;
            float dropRate = num * ratesInfo.parentDroprateChance;
            ratesInfo.conditions = new List<IItemDropRuleCondition>() { this, };
            drops.Add(new DropRateInfo(ItemType, 1, 1, dropRate, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            var result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.FailedRandomRoll;

            if (Condition.CanDrop(info) || info.rng.NextBool(Chance)) {
                CommonCode.DropItem(info, ItemType, 1);
                result.State = ItemDropAttemptResultState.Success;
            }
            return result;
        }

        public bool CanDrop(DropAttemptInfo info) {
            return true;
        }

        public bool CanShowItemDropInUI() {
            return true;
        }

        public string GetConditionDescription() {
            string conditionDesc = Condition?.GetConditionDescription();
            return conditionDesc != null ? TextHelper.GetTextValue("DropCondition.OtherwiseChance", conditionDesc) : null;
        }
    }
}