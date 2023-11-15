using Aequus.Common.Items.Components;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public class TransformGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is ITransformItem;
    }

    public override void HoldItem(Item item, Player player) {
        if (item.ModItem is ITransformItem transformItem && Main.myPlayer == player.whoAmI && Main.mouseRight && Main.mouseRightRelease && !Main.LocalPlayer.lastMouseInterface && Main.LocalPlayer.ItemTimeIsZero) {
            transformItem.HoldItemTransform(player);
        }
    }
}