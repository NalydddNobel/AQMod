using System.Collections.Generic;

namespace Aequus.Common.Items.Chests;

internal record class CommonChestRule(int Item, int MinStack = 1, int MaxStack = 1) : IChestLootRule {
    public List<IChestLootChain> ChainedRules { get; set; } = new();

    public ChestLootResult AddItem(in ChestLootInfo info) {
        Main.chest[info.Chest].AddItem(Item, info.RNG.Next(MinStack, MaxStack + 1));
        return ChestLootResult.Success;
    }

    public bool CanDrop(in ChestLootInfo info) {
        return true;
    }
}
