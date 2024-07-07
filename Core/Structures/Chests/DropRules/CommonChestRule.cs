using AequusRemake.Core.Structures.Conversion;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Terraria;

namespace AequusRemake.DataSets.Structures.DropRulesChest;

public record class CommonChestRule(int Item, int MinStack = 1, int MaxStack = 1, int ChanceDenominator = 1, int ChanceNumerator = 1, params Condition[] OptionalConditions)
    : IChestLootRule, IConvertDropRules {
    public List<IChestLootChain> ChainedRules { get; set; } = new();
    public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

    public ChestLootResult AddItem(in ChestLootInfo info) {
        // Roll RNG.
        if (info.RNG.Next(ChanceDenominator) >= ChanceNumerator) {
            return ChestLootResult.FailedRandomRoll;
        }

        // Add the item.
        info.Add.AddItem(Item, info.RNG.Next(MinStack, MaxStack + 1), in info);
        return ChestLootResult.Success;
    }

    public IChestLootRule ToChestDropRule() {
        return this;
    }

    public IItemDropRule ToItemDropRule() {
        return new CommonDrop(Item, ChanceDenominator, MinStack, MaxStack, ChanceNumerator);
    }
}
