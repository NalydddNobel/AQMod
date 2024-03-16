using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace Aequus.Common.Items.Chests;

public interface IChestLootRule {
    List<IChestLootChain> ChainedRules { get; }

    bool CanDrop(in ChestLootInfo info);

    ChestLootResult AddItem(in ChestLootInfo info);
}

public interface IChestLootChain {
    IChestLootRule RuleToChain { get; }

    bool CanChainIntoRule(ChestLootResult parentResult);
}

public readonly record struct ChestLootResult(ItemDropAttemptResultState State) {
    public static readonly ChestLootResult Success = new() { State = ItemDropAttemptResultState.Success };
    public static readonly ChestLootResult DoesntFillConditions = new() { State = ItemDropAttemptResultState.DoesntFillConditions };
}
public readonly record struct ChestLootInfo(int Chest, UnifiedRandom RNG);