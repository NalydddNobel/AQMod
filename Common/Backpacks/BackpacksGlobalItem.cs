namespace Aequus.Common.Backpacks;

public sealed class BackpacksGlobalItem : GlobalItem {
    public override System.Boolean ItemSpace(Item item, Player player) {
        if (!player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return false;
        }

        if (!BackpackLoader.IgnoreBackpacks) {
            for (System.Int32 i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (backpackPlayer.backpacks[i].IsActive(player) && BackpackLoader.ItemSpace(item, player, backpackPlayer.backpacks[i])) {
                    return true;
                }
            }
        }
        return false;
    }

    public override System.Boolean OnPickup(Item item, Player player) {
        if (!player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return true;
        }

        BackpackLoader.IgnoreBackpacks = true;
        var itemSpace = player.ItemSpace(item);
        BackpackLoader.IgnoreBackpacks = false;
        return !BackpackLoader.GrabItem(item, player, backpackPlayer.backpacks, itemSpace);
    }
}