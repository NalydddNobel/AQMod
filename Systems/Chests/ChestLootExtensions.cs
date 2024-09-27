using Aequus.Systems.Chests.DropRules;

namespace Aequus.Systems.Chests;

public static class ChestLootExtensions {
    public static void RegisterCommon(this ChestLootDatabase database, ChestPool type, int item, int minStack = 1, int maxStack = 1, int chanceDenominator = 1, int chanceNumerator = 1, params Condition[] conditions) {
        database.Register(type, new CommonChestRule(item, minStack, maxStack, chanceDenominator, chanceNumerator, conditions));
    }

    public static void RegisterIndexed(this ChestLootDatabase database, int chanceDenominator, ChestPool type, params IChestLootRule[] rules) {
        database.Register(type, new IndexedChestRule(chanceDenominator, rules));
    }

    public static void RegisterOneFromOptions(this ChestLootDatabase database, int chanceDemoninator, ChestPool type, params IChestLootRule[] rules) {
        database.Register(type, new OneFromOptionsChestRule(rules, chanceDemoninator));
    }

    public static IChestLootRule OnSucceed(this IChestLootRule parentRule, IChestLootRule rule) {
        parentRule.ChainedRules.Add(new OnSuccessChestChain(rule));
        return parentRule;
    }

    public static IChestLootRule OnFailure(this IChestLootRule parentRule, IChestLootRule rule) {
        parentRule.ChainedRules.Add(new OnFailChestChain(rule));
        return parentRule;
    }

    public static bool CanDropWithConditions(this IChestLootRule rule, in ChestLootInfo info) {
        return rule.CanDrop(in info) && (rule.Conditions?.IsMet() == false ? false : true);
    }

    public static void ClearSelfAndChains(this IChestLootRule rule) {
        rule.Reset();
        foreach (var c in rule.ChainedRules) {
            c.RuleToChain?.ClearSelfAndChains();
        }
    }
}