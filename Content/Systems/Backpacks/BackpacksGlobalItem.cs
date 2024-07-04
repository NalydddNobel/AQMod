namespace AequusRemake.Content.Backpacks;

public sealed class BackpacksGlobalItem : GlobalItem {
    public override bool ItemSpace(Item item, Player player) {
        if (!player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return false;
        }

        if (!BackpackLoader.IgnoreBackpacks) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (backpackPlayer.backpacks[i].IsActive(player) && BackpackLoader.ItemSpace(item, player, backpackPlayer.backpacks[i])) {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool OnPickup(Item item, Player player) {
        if (!player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return true;
        }

        BackpackLoader.IgnoreBackpacks = true;
        var itemSpace = player.ItemSpace(item);
        BackpackLoader.IgnoreBackpacks = false;
        return !BackpackLoader.GrabItem(item, player, backpackPlayer.backpacks, itemSpace);
    }
}