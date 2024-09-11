using Aequus.Common.Structures.Conditions;
using Aequus.Systems.Chests.DropRules;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean.Generation;

public class RandomTrashChestRule(int[] ItemTypesToSpawn, int MinPerItem, int MaxPerItem, int MinTotalQuantity, int MaxTotalQuantity, params Condition[] OptionalConditions) : IChestLootRule {
    public List<IChestLootChain> ChainedRules { get; set; } = [];
    public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

    public ChestLootResult AddItem(in ChestLootInfo info) {
        Chest chest = Main.chest[info.ChestId];
        int quantity = info.RNG.Next(MinTotalQuantity, MaxTotalQuantity);

        while (quantity > 0) {
            int randomIndex = info.RNG.Next(chest.item.Length);
            int nextItemType = info.RNG.Next(ItemTypesToSpawn);
            int nextItemStack = Math.Min(info.RNG.Next(MinPerItem, MaxPerItem + 1), quantity);

            Item nextItem = new(nextItemType, stack: nextItemStack);
            chest.InsertItem(nextItem, randomIndex);

            quantity -= nextItemStack;
        }

        return ChestLootResult.Success;
    }
}
