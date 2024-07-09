using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AequusRemake.Systems.Fishing;

public class FishLootDatabase {
    public static readonly FishLootDatabase Instance = new();

    public readonly Dictionary<CatchTier, List<IFishDropRule>> Rules = [];

    public void Add(IFishDropRule rule, CatchTier tier = CatchTier.None) {
        (CollectionsMarshal.GetValueRefOrAddDefault(Rules, tier, out _) ??= []).Add(rule);
    }

    public void Solve(ref FishDropInfo dropInfo) {
        for (CatchTier i = CatchTier.None; i < CatchTier.Count; i++) {
            if (HasTier(in dropInfo, i) && Rules.TryGetValue(i, out List<IFishDropRule> rules)) {
                foreach (IFishDropRule rule in rules) {
                    if (rule.CanCatch(in dropInfo)) {
                        rule.TryCatching(ref dropInfo);
                    }
                }
            }
        }
    }

    public static bool HasTier(in FishDropInfo dropInfo, CatchTier tier) {
        return tier switch {
            CatchTier.Legendary => dropInfo.Attempt.legendary,
            CatchTier.VeryRare => dropInfo.Attempt.veryrare,
            CatchTier.Rare => dropInfo.Attempt.rare,
            CatchTier.Uncommon => dropInfo.Attempt.uncommon,
            CatchTier.Common => dropInfo.Attempt.common,
            _ => true
        };
    }
}
