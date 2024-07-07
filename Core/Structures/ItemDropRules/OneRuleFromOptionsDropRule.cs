using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Core.Structures.ItemDropRules;

public record class OneRuleFromOptionsDropRule(IItemDropRule[] Options, int ChanceDenominator = 1, int ChanceNumerator = 1) : IItemDropRule {
    public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; } = new();

    public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
        // Check rng roll for this rule.

        //if (info.player.RollLuck(ChanceDenominator) >= ChanceNumerator) {
        if (info.rng.Next(ChanceDenominator) >= ChanceNumerator) {
            return new ItemDropAttemptResult() { State = ItemDropAttemptResultState.FailedRandomRoll };
        }

        IItemDropRule rule = info.rng.Next(Options);

        // Check rolled rule conditions.
        if (!rule.CanDrop(info)) {
            return new ItemDropAttemptResult() { State = ItemDropAttemptResultState.DoesntFillConditions };
        }

        // Run inner rule drop code.
        return rule.TryDroppingItem(info);
    }

    public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
        float personalDropRate = ChanceNumerator / (float)ChanceDenominator;
        float realDropRate = personalDropRate * ratesInfo.parentDroprateChance;

        float dropRatePerItem = 1f / Options.Length * realDropRate;
        DropRateInfoChainFeed rateInfoPerItem = new DropRateInfoChainFeed(dropRatePerItem);

        // Report each option's droprates, modified using this rule's RNG chance.
        for (int i = 0; i < Options.Length; i++) {
            Options[i].ReportDroprates(drops, rateInfoPerItem);
        }

        Chains.ReportDroprates(ChainedRules, personalDropRate, drops, ratesInfo);
    }

    public bool CanDrop(DropAttemptInfo info) {
        return true;
    }
}
