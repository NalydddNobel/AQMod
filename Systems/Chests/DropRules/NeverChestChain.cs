namespace AequusRemake.Systems.Chests.DropRules;

public class NeverChestChain : IChestLootChain {
    public IChestLootRule RuleToChain { get; private set; }

    public NeverChestChain(IChestLootRule rule) {
        RuleToChain = rule;
    }

    public bool CanChainIntoRule(ChestLootResult parentResult) {
        return false;
    }
}
