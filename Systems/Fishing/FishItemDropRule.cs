using AequusRemake.Core.Structures.Conversion;
using AequusRemake.DataSets.Structures.DropRulesChest;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Systems.Fishing;

public record class FishItemDropRule(int Item, int MinStack = 1, int MaxStack = 1, int ChanceDenominator = 1, int ChanceNumerator = 1, Condition Condition = null) : IFishDropRule, IConvertDropRules {
    bool IFishDropRule.CanCatch(in FishDropInfo dropInfo) {
        return Condition?.IsMet() ?? true;
    }

    void IFishDropRule.TryCatching(ref FishDropInfo dropInfo) {
        if (dropInfo.RNG.Next(ChanceDenominator) < ChanceNumerator) {
            dropInfo.Item = Item;
        }
    }

    IChestLootRule IConvertDropRules.ToChestDropRule() {
        return new CommonChestRule(Item, MinStack, MaxStack, ChanceDenominator, ChanceNumerator, Condition);
    }

    IItemDropRule IConvertDropRules.ToItemDropRule() {
        return new CommonDrop(Item, MinStack, MaxStack, ChanceDenominator, ChanceNumerator);
    }

    IFishDropRule IConvertDropRules.ToFishDropRule() {
        return this;
    }
}
