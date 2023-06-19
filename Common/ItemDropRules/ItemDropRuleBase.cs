using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDropRules {
    public abstract class ItemDropRuleBase : IItemDropRule {
        public List<IItemDropRuleChainAttempt> ChainedRules { get; set; }

        public virtual bool CanDrop(DropAttemptInfo info) {
            return true;
        }

        public ItemDropRuleBase() {
            ChainedRules = new();
        }

        public abstract void ReportDroprates(List<DropRateInfo> drops, ref DropRateInfoChainFeed ratesInfo, ref float personalDropRate);

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
            float chance = 1f;
            ReportDroprates(drops, ref ratesInfo, ref chance);
            Chains.ReportDroprates(ChainedRules, chance, drops, ratesInfo);
        }

        public abstract ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info);
    }
    public abstract class ItemDropRuleChanceBase : ItemDropRuleBase {
        public int chanceDenominator;
        public int chanceNumerator;

        public ItemDropRuleChanceBase(int numerator, int demoninator = 1) : base() {
            chanceNumerator = numerator;
            chanceDenominator = demoninator;
        }

        public override void ReportDroprates(List<DropRateInfo> drops, ref DropRateInfoChainFeed ratesInfo, ref float personalDropRate) {
            personalDropRate = chanceNumerator / (float)chanceDenominator;
        }
    }
}
