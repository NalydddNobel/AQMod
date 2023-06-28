using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules {
    public class OneFromOptionsWithStackWithCondition : IItemDropRule {
        public ItemDrop[] itemDrops;
        public int chanceDenominator;
        public int chanceNumerator;
        public IItemDropRuleCondition condition;

        public List<IItemDropRuleChainAttempt> ChainedRules {
            get;
            private set;
        }

        public OneFromOptionsWithStackWithCondition(IItemDropRuleCondition condition = null, params ItemDrop[] options) : this(1, 1, condition, options) {
        }

        public OneFromOptionsWithStackWithCondition(int chanceDenominator, int chanceNumerator, IItemDropRuleCondition condition = null, params ItemDrop[] options) {
            this.chanceDenominator = chanceDenominator;
            this.chanceNumerator = chanceNumerator;
            itemDrops = options;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
            this.condition = condition;
        }

        public bool CanDrop(DropAttemptInfo info) {
            return condition == null || condition.CanDrop(info);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(chanceDenominator) < chanceNumerator) {
                int chosen = info.rng.Next(itemDrops.Length);
                CommonCode.DropItem(info, itemDrops[chosen].item, itemDrops[chosen].RollStack(info.rng));
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
            var ratesInfo2 = ratesInfo.With(1f);
            if (condition != null) {
                ratesInfo2.AddCondition(condition);
            }
            float num = chanceNumerator / (float)chanceDenominator;
            float num2 = num * ratesInfo2.parentDroprateChance;
            float dropRate = 1f / itemDrops.Length * num2;
            for (int i = 0; i < itemDrops.Length; i++) {
                drops.Add(new DropRateInfo(itemDrops[i].item, itemDrops[i].minStack, itemDrops[i].maxStack, dropRate, ratesInfo2.conditions));
            }

            Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo2);
        }
    }
}