using Terraria.GameContent.ItemDropRules;

namespace Aequus.DataSets.Structures.DropRulesChest;

public class OnSuccessChestChain : IChestLootChain {
    public IChestLootRule RuleToChain { get; private set; }

    public OnSuccessChestChain(IChestLootRule rule) {
        RuleToChain = rule;
    }

    public bool CanChainIntoRule(ChestLootResult parentResult) {
        return parentResult.State == ItemDropAttemptResultState.Success;
    }
}
