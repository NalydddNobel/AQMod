using AequusRemake.Core.Entities.Items;
using System;
using System.Collections.Generic;
using tModLoaderExtended.Terraria;

namespace AequusRemake.DataSets.Structures.DropRulesChest;

public record class MetalBarHackChestRule(Func<int> GetOreTileId, int MinStack = 1, int MaxStack = 1, params Condition[] OptionalConditions) : IChestLootRule {
    public List<IChestLootChain> ChainedRules { get; set; } = new();
    public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

    public ChestLootResult AddItem(in ChestLootInfo info) {
        int scannedItem = GetItem();

        if (scannedItem > 0) {
            // Add the item.
            info.Add.AddItem(scannedItem, info.RNG.Next(MinStack, MaxStack + 1), in info);
            return ChestLootResult.Success;
        }

        return ChestLootResult.DidNotRunCode;
    }

    private int GetItem() {
        int tileId = GetOreTileId();
        return ItemScanner.GetBarFromTileId(tileId);
    }
}
