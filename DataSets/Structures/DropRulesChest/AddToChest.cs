namespace Aequus.DataSets.Structures.DropRulesChest;

public interface IAddToChest {
    void AddItem(int item, int stack, in ChestLootInfo info, int prefix = -1);
    void RemoveItem(in ChestLootInfo info) {
        throw new System.NotImplementedException();
    }
}

public struct AddToChest : IAddToChest {
    public void AddItem(int item, int stack, in ChestLootInfo info, int prefix = -1) {
        Main.chest[info.ChestId].AddItem(item, stack, prefix);
    }
}

public readonly record struct InsertToChest(int Slot) : IAddToChest {
    public void AddItem(int item, int stack, in ChestLootInfo info, int prefix = -1) {
        Main.chest[info.ChestId].InsertItem(new Item(item, stack, prefix), Slot);
    }

    public void RemoveItem(in ChestLootInfo info) {
        Chest chest = Main.chest[info.ChestId];
        Main.chest[info.ChestId].Remove(Slot);
    }
}