using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Systems.Chests.DropRules;

public class ChainTryIfSucceed : IChestLootChain {
    public IChestLootRule RuleToChain { get; private set; }

    public ChainTryIfSucceed(IChestLootRule rule) {
        RuleToChain = rule;
    }

    public bool CanChainIntoRule(ChestLootResult parentResult) {
        return parentResult.State == ItemDropAttemptResultState.Success;
    }
}

