namespace AequusRemake.Systems.Chests.DropRules;

public class AlwaysChestChain : IChestLootChain {
    public IChestLootRule RuleToChain { get; private set; }

    public AlwaysChestChain(IChestLootRule rule) {
        RuleToChain = rule;
    }

    public bool CanChainIntoRule(ChestLootResult parentResult) {
        return true;
    }
}
