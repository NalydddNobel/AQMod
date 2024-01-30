namespace Aequus.Common.Items.Components;

public interface ITransformItem {
    void Transform(Player player);

    void HoldItemTransform(Player player) {
        Transform(player);
    }

    void SlotTransform(Item[] inventory, System.Int32 context, System.Int32 slot) {
        Transform(Main.LocalPlayer);
    }
}

internal sealed class TransformGlobalItem : GlobalItem {
    public override System.Boolean AppliesToEntity(Item entity, System.Boolean lateInstantiation) {
        return entity.ModItem is ITransformItem;
    }

    public override void HoldItem(Item item, Player player) {
        if (item.ModItem is ITransformItem transformItem && Main.myPlayer == player.whoAmI && Main.mouseRight && Main.mouseRightRelease && !Main.LocalPlayer.lastMouseInterface && Main.LocalPlayer.ItemTimeIsZero) {
            transformItem.HoldItemTransform(player);
        }
    }
}