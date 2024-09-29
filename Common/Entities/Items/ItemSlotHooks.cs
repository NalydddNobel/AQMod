using Terraria.UI;

namespace Aequus.Common.Entities.Items;

public interface IPickItemSlotMovementAction {
    public const int RESULT_NONE = -1;
    /// <summary>Returning this result will attempt to place the item into the slot.</summary>
    public const int RESULT_SIMPLE_SWAP = 0;
    /// <summary>Returning this result will attempt to place the item into the slot, if it is an equipable and follows conditions depending on the context.</summary>
    public const int RESULT_EQUIPMENT_SWAP = 1;
    /// <summary>Returning this result will attempt to place the item into the slot, if it is a Dye.</summary>
    public const int RESULT_DYE_SWAP = 2;
    /// <summary>Returning this result will attempt to purchase the item.</summary>
    public const int RESULT_BUY_ITEM = 3;
    /// <summary>Returning this result will attempt to sell the item.</summary>
    public const int RESULT_SELL_ITEM = 4;
    /// <summary>Returning this result will attempt dupe the item from the item slot, stacking it onto the mouse item.</summary>
    public const int RESULT_JOURNEY_DUPE = 5;

    void OverrideItemMovementAction(ref int result, Item[] inventory, int context, int slot, Item checkItem);
}

internal interface IItemSlotOverride {
    /// <returns>true to prevent vanilla right click actions.</returns>
    bool RightClickSlot(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus);
}

internal interface IItemSlotOverrideWhileHeldInMouse {
    /// <returns>true to prevent vanilla right click actions.</returns>
    bool RightClickSlot(ref Item heldItem, Item[] inv, int context, int slot, Player player);
}

internal class ItemSlotHooks : LoadedType {
    protected override void Load() {
        On_ItemSlot.PickItemMovementAction += On_ItemSlot_PickItemMovementAction;
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick;
    }

    static int On_ItemSlot_PickItemMovementAction(On_ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem) {
        int result = orig(inv, context, slot, checkItem);

        if (checkItem?.ModItem is IPickItemSlotMovementAction pickItemMovementAction) {
            pickItemMovementAction.OverrideItemMovementAction(ref result, inv, context, slot, checkItem);
        }

        return result;
    }

    static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (Main.mouseRight && Main.mouseRightRelease) {
            Player player = Main.LocalPlayer;
            AequusPlayer aequus = player.GetModPlayer<AequusPlayer>();

            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is IItemSlotOverrideWhileHeldInMouse hold && hold.RightClickSlot(ref Main.mouseItem, inv, context, slot, player)) {
                Main.mouseRightRelease = false;
                // Set stack split delay to 3 seconds (so you don't instantly pick up the item with rclick)
                Main.stackSplit = 180;
                return;
            }

            if (!inv[slot].IsAir && inv[slot].ModItem is IItemSlotOverride slotOverride && slotOverride.RightClickSlot(ref Main.mouseItem, inv, context, slot, player, aequus)) {
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