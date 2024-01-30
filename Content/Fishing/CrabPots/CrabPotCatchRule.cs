using Terraria.Utilities;

namespace Aequus.Content.Fishing.CrabPots;

public record struct CrabPotCatchRule(System.Int32 ItemId, System.Int32 ChanceDenominator, System.Int32 ChanceNumerator = 1, System.Int32 MinStack = 1, System.Int32 MaxStack = 1, CrabPotCatchRule.CrabPotCondition Condition = null) {
    public delegate System.Boolean CrabPotCondition(System.Int32 x, System.Int32 y);

    public System.Boolean PassesCondition(System.Int32 x, System.Int32 y) {
        return Condition?.Invoke(x, y) != false;
    }

    public System.Boolean RollChance(UnifiedRandom random) {
        return random.Next(ChanceDenominator) <= ChanceNumerator;
    }

    public System.Int32 RollStack(UnifiedRandom random) {
        return random.Next(MinStack, MaxStack);
    }
}