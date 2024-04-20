using Aequus.Common.Chests;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.DataSets;

/// <summary>Replica of <see cref="ItemDropDatabase"/>/<see cref="ItemDropResolver"/>, except for Aequus' chest loot.</summary>
public class ChestLootDatabase : ILoad {
    public static ChestLootDatabase Instance { get; private set; }

    private readonly Dictionary<ChestLoot, List<IChestLootRule>> _loot = new();

    public void Register(ChestLoot type, IChestLootRule rule) {
        (CollectionsMarshal.GetValueRefOrAddDefault(_loot, type, out _) ??= new()).Add(rule);
    }

    public List<IChestLootRule> GetRulesForType(ChestLoot type) {
        return CollectionsMarshal.GetValueRefOrAddDefault(_loot, type, out _) ??= new();
    }

    public void SolveRules(ChestLoot type, ChestLootInfo info) {
        SolveRules(type, in info);
    }

    public void SolveRules(ChestLoot type, in ChestLootInfo info) {
        List<IChestLootRule> rules = Instance.GetRulesForType(type);

        if (rules != null) {
            for (int i = 0; i < rules.Count; ++i) {
                SolveSingleRule(rules[i], in info);
            }
        }
    }

    public ChestLootResult SolveSingleRule(IChestLootRule rule, in ChestLootInfo info) {
        ChestLootResult result;

        if (!rule.CanDropWithConditions(info)) {
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
                SolveSingleRule(chain.RuleToChain, info);
            }
        }
    }

    internal void OnClearWorld() {
        if (_loot == null) {
            return;
        }

        foreach (List<IChestLootRule> rules in _loot.Values) {
            if (rules == null) {
                continue;
            }

            foreach (IChestLootRule rule in rules) {
                rule.ClearSelfAndChains();
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
