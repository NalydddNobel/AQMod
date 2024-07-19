using Aequus.Common.Structures.Conditions;
using Aequus.Systems.Chests;
using System.Collections.Generic;

namespace Aequus.Systems.Chests.DropRules;

/// <summary>
/// Deletes the first slot, and runs <paramref name="Rule"/> on its slot with an <see cref="InsertToChest"/> context.
/// <para>
/// To remove an item properly, use <see cref="RemoveItemChestRule"/> as the only <paramref name="Rule"/> to pull all of the slots backwards upon deleting the item.
/// </para>
/// </summary>
public record class ReplaceFirstSlotChestRule(IChestLootRule Rule, params Condition[] OptionalConditions) : IChestLootRule {
    public List<IChestLootChain> ChainedRules { get; set; } = IChestLootChain.GetFromSelfRules(Rule);
    public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

    public ChestLootResult AddItem(in ChestLootInfo info) {
        Chest chest = Main.chest[info.ChestId];
        // Delete item
        chest.item[0].TurnToAir();

        // Run Rule with "Insert" properties.
        ChestLootInfo childInfo = new ChestLootInfo(info, new InsertToChest(0));
        ChestLootDatabase.Instance.SolveSingleRule(Rule, in childInfo);

        // Success!!
        return ChestLootResult.Success;
    }
}
