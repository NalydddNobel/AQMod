namespace Aequus.Systems.Chests.DropRules;

public readonly struct AddToChest : IAddToChest {
    public void AddItem(int item, int stack, in ChestLootInfo info, int prefix = -1) {
        Main.chest[info.ChestId].AddItem(item, stack, prefix);
    }
}
