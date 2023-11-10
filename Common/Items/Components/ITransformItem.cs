using Terraria;

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