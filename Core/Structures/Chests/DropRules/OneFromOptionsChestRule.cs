using AequusRemake.Core.Structures.Chests;
using AequusRemake.Core.Structures.Conversion;
using AequusRemake.Core.Structures.ItemDropRules;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Terraria;

namespace AequusRemake.DataSets.Structures.DropRulesChest;

public record class OneFromOptionsChestRule(IChestLootRule[] Options, int ChanceDenominator = 1, int ChanceNumerator = 1, params Condition[] OptionalConditions)
    : IChestLootRule, IConvertDropRules {
    public List<IChestLootChain> ChainedRules { get; set; } = IChestLootChain.GetFromSelfRules(Options);
    public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

    public ChestLootResult AddItem(in ChestLootInfo info) {
        // Roll RNG.
        if (info.RNG.Next(ChanceDenominator) >= ChanceNumerator) {
            return ChestLootResult.FailedRandomRoll;
        }

        // Roll a random rule 
        IChestLootRule selectedRule = info.RNG.Next(Options);

        // Solve that rule.
        ChestLootResult result = ChestLootDatabase.Instance.SolveSingleRule(selectedRule, in info);

        return result;
    }

    public IChestLootRule ToChestDropRule() {
        return this;
    }

    public IItemDropRule ToItemDropRule() {
        return new OneRuleFromOptionsDropRule(Options.SelectWhereOfType<IConvertDropRules>()
            .Select(convert => convert.ToItemDropRule())
            .ToArray(), ChanceDenominator, ChanceNumerator);
    }
}
