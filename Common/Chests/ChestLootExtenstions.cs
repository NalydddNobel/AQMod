using Aequus.DataSets;

namespace Aequus.Common.Chests;

public static class ChestLootExtenstions {
    public static void RegisterCommon(this ChestLootDatabase database, ChestLoot type, int item, int minStack = 1, int maxStack = 1, int chanceDemoninator = 1, int chanceNumerator = 1, params Condition[] conditions) {
        database.Register(type, new ChestRules.Common(item, minStack, maxStack, chanceDemoninator, chanceNumerator, conditions));
    }

    public static void RegisterIndexed(this ChestLootDatabase database, ChestLoot type, params IChestLootRule[] rules) {
        database.Register(type, new ChestRules.Indexed(rules));
    }

    public static void RegisterOneFromOptions(this ChestLootDatabase database, ChestLoot type, params IChestLootRule[] rules) {
        database.Register(type, new ChestRules.OneFromOptions(rules));
    }

    public static IChestLootRule OnSucceed(this IChestLootRule parentRule, IChestLootRule rule) {
        parentRule.ChainedRules.Add(new ChestChains.OnSuccess(rule));
        return parentRule;
    }

    public static IChestLootRule OnFailure(this IChestLootRule parentRule, IChestLootRule rule) {
        parentRule.ChainedRules.Add(new ChestChains.OnFailure(rule));
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