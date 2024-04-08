using Aequus.Common.Items.Components;
using Terraria.UI;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (Main.mouseRight && Main.mouseRightRelease) {
            var player = Main.LocalPlayer;
            var aequus = player.GetModPlayer<AequusPlayer>();
            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is IRightClickOverrideWhenHeld rightClickOverride && rightClickOverride.RightClickOverrideWhileHeld(ref Main.mouseItem, inv, context, slot, player, aequus)) {
                Main.mouseRightRelease = false;
                // Set stack split delay to 3 seconds (so you don't instantly pick up the item with rclick)
                Main.stackSplit = 180;
                return;
            }

            if (context == ItemSlot.Context.InventoryItem) {
                if (aequus.UseGoldenKey(inv, slot)) {
                    return;
                }
                if (aequus.UseShadowKey(inv, slot)) {
                    return;
                }
            }
        }

        orig(inv, context, slot);
    }
}
