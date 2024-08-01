using System.Reflection;

namespace Aequus.Common.Utilities.Helpers;

public sealed class InventoryTools : ModSystem {
    private readonly MethodInfo GetItem_FillEmptyInventorySlot = typeof(Player).GetMethod("GetItem_FillEmptyInventorySlot")!;

    public bool PlaceItemInEmptyInventorySlot(int plr, Item newItem, GetItemSettings settings, Item returnItem, int i) {
        return (GetItem_FillEmptyInventorySlot.Invoke(null, [plr, newItem, settings, returnItem, i]) as bool?) ?? false;
    }
}
