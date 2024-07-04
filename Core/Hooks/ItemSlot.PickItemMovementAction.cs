using AequusRemake.Core.Entities.Items.Components;
using Terraria.UI;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows custom item movement action overrides for hovered modded items which implement <see cref="IPickItemMovementAction"/>.</summary>
    private static int On_ItemSlot_PickItemMovementAction(On_ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem) {
        int result = orig(inv, context, slot, checkItem);

        if (checkItem?.ModItem is IPickItemMovementAction pickItemMovementAction) {
            pickItemMovementAction.OverrideItemMovementAction(ref result, inv, context, slot, checkItem);
        }

        return result;
    }
}
