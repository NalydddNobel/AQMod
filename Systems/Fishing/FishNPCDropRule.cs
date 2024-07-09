namespace AequusRemake.Systems.Fishing;

public record class FishNPCDropRule(int NPC, int ChanceDenominator = 1, int ChanceNumerator = 1, Condition Condition = null) : IFishDropRule {
    bool IFishDropRule.CanCatch(in FishDropInfo dropInfo) {
        return Condition?.IsMet() ?? true;
    }

    void IFishDropRule.TryCatching(ref FishDropInfo dropInfo) {
        if (dropInfo.RNG.Next(ChanceDenominator) < ChanceNumerator) {
            dropInfo.NPC = NPC;
        }
    }
}