using Aequus.Common.Utilities.Extensions;

namespace Aequus.Systems.Chests.DropRules;

public readonly record struct InsertToChest(int Slot) : IAddToChest {
    public void AddItem(int item, int stack, in ChestLootInfo info, int prefix = -1) {
        Main.chest[info.ChestId].Insert(new Item(item, stack, prefix), Slot);
    }

    public void RemoveItem(in ChestLootInfo info) {
        Chest chest = Main.chest[info.ChestId];
        Main.chest[info.ChestId].Remove(Slot);
    }
}