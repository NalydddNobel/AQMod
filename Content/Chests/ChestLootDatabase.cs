using Aequus.Common.Items.Chests;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Chests;

/// <summary>Replica of <see cref="ItemDropDatabase"/>/<see cref="ItemDropResolver"/>, except for Aequus' chest loot.</summary>
public class ChestLootDatabase : ILoad {
    public static ChestLootDatabase Instance { get; private set; }

    private readonly Dictionary<ChestLoot, List<IChestLootRule>> _loot = new();

    public void Register(ChestLoot type, IChestLootRule rule) {
        (CollectionsMarshal.GetValueRefOrAddDefault(_loot, type, out _) ??= new()).Add(rule);
    }

    public List<IChestLootRule> GetRulesForType(ChestLoot type) {
        return _loot[type];
    }

    public ChestLootResult SolveIndexedRule(List<IChestLootRule> rules, in ChestLootInfo info, ref int index) {
        int nextRuleIndex;
        ChestLootResult result;
        do {
            nextRuleIndex = index++ % rules.Count;
            IChestLootRule rule = rules[nextRuleIndex];
            result = SolveRule(rule, in info);
        }
        while (result.State != ItemDropAttemptResultState.Success);
        return result;
    }

    public ChestLootResult SolveRule(IChestLootRule rule, in ChestLootInfo info) {
        ChestLootResult result;

        if (!rule.CanDrop(info)) {
            result = ChestLootResult.DoesntFillConditions;
        }
        else {
            result = rule.AddItem(in info);
        }

        ResolveRuleChains(rule, in info, in result);
        return result;
    }

    private void ResolveRuleChains(IChestLootRule rule, in ChestLootInfo info, in ChestLootResult parentResult) {
        ResolveRuleChains(in info, in parentResult, rule.ChainedRules);
    }

    private void ResolveRuleChains(in ChestLootInfo info, in ChestLootResult parentResult, List<IChestLootChain> ruleChains) {
        if (ruleChains == null) {
            return;
        }

        for (int i = 0; i < ruleChains.Count; i++) {
            IChestLootChain chain = ruleChains[i];
            if (chain.CanChainIntoRule(parentResult)) {
                SolveRule(chain.RuleToChain, info);
            }
        }
    }

    public void Load(Mod mod) {
        Instance = this;
    }

    public void Unload() {
        Instance = null;
    }
}

public enum ChestLoot {
    PollutedOcean,
    HardmodeChestRegular,
    HardmodeChestSnow,
    HardmodeChestJungle,
}

public static class ChestLootExtenstions {
    public static void RegisterCommon(this ChestLootDatabase database, ChestLoot type, int item, int minStack = 1, int maxStack = 1) {
        database.Register(type, new CommonChestRule(item, minStack, maxStack));
    }
    public static IChestLootRule OnSucceed(this IChestLootRule parentRule, IChestLootRule rule) {
        parentRule.ChainedRules.Add(new ChainTryIfSucceed(rule));
        return rule;
    }
}