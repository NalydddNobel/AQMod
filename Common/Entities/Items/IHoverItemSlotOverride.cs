using Terraria.UI;

namespace Aequus.Common.Entities.Items;

internal interface IHoverItemSlotOverride {
    /// <returns>true to prevent vanilla right click actions.</returns>
    bool RightClickSlot(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus);
}

internal class RightClickHoverHook : LoadedType {
    protected override void Load() {
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick;
    }

    private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (Main.mouseRight && Main.mouseRightRelease) {
            Player player = Main.LocalPlayer;
            AequusPlayer aequus = player.GetModPlayer<AequusPlayer>();

            /*
            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is IRightClickOverrideWhenHeld hold && hold.RightClickOverrideWhileHeld(ref Main.mouseItem, inv, context, slot, player, aequus)) {
                Main.mouseRightRelease = false;
                // Set stack split delay to 3 seconds (so you don't instantly pick up the item with rclick)
                Main.stackSplit = 180;
                return;
            }
            */

            if (!inv[slot].IsAir && inv[slot].ModItem is IHoverItemSlotOverride hover && hover.RightClickSlot(ref Main.mouseItem, inv, context, slot, player, aequus)) {
                Main.mouseRightRelease = false;
                // Set stack split delay to 3 seconds (so you don't instantly pick up the item with rclick)
                Main.stackSplit = 180;
                return;
            }

            // Custom Golden Key / Shadow Key code for lockboxes.

            /*
            if (context == ItemSlot.Context.InventoryItem) {
                if (UseGoldenKey(inv, slot, player, aequus)) {
                    return;
                }
                if (UseShadowKey(inv, slot, player, aequus)) {
                    return;
                }
            }
            */
        }

        orig(inv, context, slot);
    }
}
