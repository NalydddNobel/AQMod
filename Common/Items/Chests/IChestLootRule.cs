using Aequus.Core;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace Aequus.Common.Items.Chests;

public interface IChestLootRule {
    List<IChestLootChain> ChainedRules { get; }
    ConditionCollection Conditions { get; }

    /// <summary>Add the item into the chest here. <see cref="ChestLootInfo.Chest"/> should safely index into <see cref="Main.chest"/>.</summary>
    ChestLootResult AddItem(in ChestLootInfo info);

    /// <summary>Condition for whether this rule should be ran.</summary>
    bool CanDrop(in ChestLootInfo info) { return true; }

    /// <summary>Called upon <see cref="ModSystem.ClearWorld"/> and Hardmode Chests. Use to reset values before world generation and etc.</summary>
    void Reset() { }
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