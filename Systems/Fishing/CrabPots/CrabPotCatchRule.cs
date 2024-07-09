using Terraria.Utilities;

namespace AequusRemake.Systems.Fishing.CrabPots;

public record struct CrabPotCatchRule(int ItemId, int ChanceDenominator, int ChanceNumerator = 1, int MinStack = 1, int MaxStack = 1, CrabPotCatchRule.CrabPotCondition Condition = null) {
    public delegate bool CrabPotCondition(int x, int y);

    public bool PassesCondition(int x, int y) {
        return Condition?.Invoke(x, y) != false;
    }

    public bool RollChance(UnifiedRandom random) {
        return random.Next(ChanceDenominator) <= ChanceNumerator;
    }

    public int RollStack(UnifiedRandom random) {
        return random.Next(MinStack, MaxStack);
    }
}