using Aequus.Common.Items.Components;
using Terraria.UI;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static int On_ItemSlot_PickItemMovementAction(On_ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem) {
        int result = orig(inv, context, slot, checkItem);

        if (checkItem?.ModItem is IPickItemMovementAction pickItemMovementAction) {
            pickItemMovementAction.OverrideItemMovementAction(ref result, inv, context, slot, checkItem);
        }

        return result;
    }
}
