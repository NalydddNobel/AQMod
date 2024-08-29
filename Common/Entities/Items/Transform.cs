using System;
using Terraria.UI;

namespace Aequus.Common.Entities.Items;

public interface ITransformItem {
    void Transform(Player player);

    void HoldItemTransform(Player player) {
        Transform(player);
    }

    void SlotTransform(Item[] inventory, int context, int slot) {
        Transform(Main.LocalPlayer);
    }
}

internal sealed class TransformGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is ITransformItem;
    }

    public override void HoldItem(Item item, Player player) {
        if (item.ModItem is ITransformItem transformItem && Main.myPlayer == player.whoAmI && Main.mouseRight && Main.mouseRightRelease && !Main.LocalPlayer.lastMouseInterface && Main.LocalPlayer.ItemTimeIsZero && CombinedHooks.CanUseItem(player, item)) {
            transformItem.HoldItemTransform(player);
        }
    }
}

internal sealed class TransformItemPlayer : ModPlayer {
    public override bool HoverSlot(Item[] inventory, int context, int slot) {
        if (inventory[slot].ModItem is ITransformItem transformItem && (context == ItemSlot.Context.InventoryItem || Math.Abs(context) == ItemSlot.Context.EquipAccessory) && Main.mouseRight && Main.mouseRightRelease && Main.LocalPlayer.ItemTimeIsZero && CombinedHooks.CanUseItem(Player, inventory[slot])) {
            transformItem.SlotTransform(inventory, context, slot);
            Main.mouseRightRelease = false;
        }
        return false;

    }
}