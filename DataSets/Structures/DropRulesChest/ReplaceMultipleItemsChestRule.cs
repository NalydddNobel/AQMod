using Aequus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using tModLoaderExtended.Terraria;

namespace Aequus.DataSets.Structures.DropRulesChest;

/// <summary>
/// Finds any item which is in <paramref name="ItemIdsToReplace"/>, deletes the item, and runs <paramref name="Rule"/> on its slot with an <see cref="InsertToChest"/> context.
/// <para>
/// To remove an item properly, use <see cref="RemoveItemChestRule"/> as the only <paramref name="Rule"/> to pull all of the slots backwards upon deleting the item.
/// </para>
/// </summary>
public record class ReplaceMultipleItemsChestRule(int[] ItemIdsToReplace, IChestLootRule Rule, params Condition[] OptionalConditions) : IChestLootRule {
    public List<IChestLootChain> ChainedRules { get; set; } = IChestLootChain.GetFromSelfRules(Rule);
    public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

    public ChestLootResult AddItem(in ChestLootInfo info) {
        ChestLootResult result = ChestLootResult.DidNotRunCode;
        Chest chest = Main.chest[info.ChestId];
        for (int i = 0; i < chest.item.Length; i++) {
            if (!chest.item[i].IsAir && ItemIdsToReplace.Contains(chest.item[i].type)) {
                // Delete item
                chest.item[i].TurnToAir();

                // Run Rule with "Insert" properties.
                ChestLootInfo childInfo = new ChestLootInfo(info, new InsertToChest(i));
                ChestLootDatabase.Instance.SolveSingleRule(Rule, in childInfo);

                result = ChestLootResult.Success;
            }
        }

        return result;
    }
}
