using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Chests;

public class ChestChains {
    public class Always : IChestLootChain {
        public IChestLootRule RuleToChain { get; private set; }

        public Always(IChestLootRule rule) {
            RuleToChain = rule;
        }

        public bool CanChainIntoRule(ChestLootResult parentResult) {
            return true;
        }
    }

    public class Never : IChestLootChain {
        public IChestLootRule RuleToChain { get; private set; }

        public Never(IChestLootRule rule) {
            RuleToChain = rule;
        }

        public bool CanChainIntoRule(ChestLootResult parentResult) {
            return false;
        }
    }

    public class OnSuccess : IChestLootChain {
        public IChestLootRule RuleToChain { get; private set; }

        public OnSuccess(IChestLootRule rule) {
            RuleToChain = rule;
        }

        public bool CanChainIntoRule(ChestLootResult parentResult) {
            return parentResult.State == ItemDropAttemptResultState.Success;
        }
    }

    public class OnFailure : IChestLootChain {
        public IChestLootRule RuleToChain { get; private set; }

        public OnFailure(IChestLootRule rule) {
            RuleToChain = rule;
        }

        public bool CanChainIntoRule(ChestLootResult parentResult) {
            return parentResult.State != ItemDropAttemptResultState.Success;
        }
    }
}
