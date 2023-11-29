using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Components;

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
        if (item.ModItem is ITransformItem transformItem && Main.myPlayer == player.whoAmI && Main.mouseRight && Main.mouseRightRelease && !Main.LocalPlayer.lastMouseInterface && Main.LocalPlayer.ItemTimeIsZero) {
            transformItem.HoldItemTransform(player);
        }
    }
}