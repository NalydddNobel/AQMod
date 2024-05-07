using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;
using tModLoaderExtended.Terraria;

namespace Aequus.DataSets.Structures.DropRulesChest;

public interface IChestLootRule {
    List<IChestLootChain> ChainedRules { get; }
    ConditionCollection Conditions { get; }

    /// <summary>Add the item into the chest here. <see cref="ChestLootInfo.ChestId"/> should safely index into <see cref="Main.chest"/>.</summary>
    ChestLootResult AddItem(in ChestLootInfo info);

    /// <summary>Condition for whether this rule should be ran.</summary>
    bool CanDrop(in ChestLootInfo info) { return true; }

    /// <summary>Called upon <see cref="ModSystem.ClearWorld"/> and Hardmode Chests. Use to reset values before world generation and etc.</summary>
    void Reset() { }
}

public interface IChestLootChain {
    IChestLootRule RuleToChain { get; }

    bool CanChainIntoRule(ChestLootResult parentResult);

    public static List<IChestLootChain> GetFromSelfRules(params IChestLootRule[] Rules) {
        return Rules.Select<IChestLootRule, IChestLootChain>(r => new NeverChestChain(r)).ToList();
    }
}

public readonly record struct ChestLootResult(ItemDropAttemptResultState State) {
    public static readonly ChestLootResult Success = new() { State = ItemDropAttemptResultState.Success };
    public static readonly ChestLootResult FailedRandomRoll = new() { State = ItemDropAttemptResultState.FailedRandomRoll };
    public static readonly ChestLootResult DoesntFillConditions = new() { State = ItemDropAttemptResultState.DoesntFillConditions };
    public static readonly ChestLootResult DidNotRunCode = new() { State = ItemDropAttemptResultState.DidNotRunCode };
}

public readonly record struct ChestLootInfo(int ChestId, UnifiedRandom RNG, IAddToChest Add) {
    public ChestLootInfo(int chestId, UnifiedRandom rng = null) : this(chestId, rng, new AddToChest()) { }
    public ChestLootInfo(ChestLootInfo parentInfo, IAddToChest newAdd) : this(parentInfo.ChestId, parentInfo.RNG, newAdd) { }
}