using Aequus.Common.Players.Backpacks;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public sealed class BackpacksGlobalItem : GlobalItem {
    public override bool ItemSpace(Item item, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return false;
        }

        if (!BackpackLoader.IgnoreBackpacks) {
            for (int i = 0; i < aequusPlayer.backpacks.Length; i++) {
                if (aequusPlayer.backpacks[i].IsActive(player) && BackpackLoader.ItemSpace(item, player, aequusPlayer.backpacks[i])) {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool OnPickup(Item item, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return true;
        }

        BackpackLoader.IgnoreBackpacks = true;
        var itemSpace = player.ItemSpace(item);
        BackpackLoader.IgnoreBackpacks = false;
        return !BackpackLoader.GrabItem(item, player, aequusPlayer.backpacks, itemSpace);
    }
}