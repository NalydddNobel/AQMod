namespace Aequus.Systems.Chests.DropRules;

public interface IAddToChest {
    void AddItem(int item, int stack, in ChestLootInfo info, int prefix = -1);
    void RemoveItem(in ChestLootInfo info) {
        throw new System.NotImplementedException();
    }
}
