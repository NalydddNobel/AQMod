using AequusRemake.Content.Backpacks;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for backpacks to provide mount items.</summary>
    private static Item On_Player_QuickMount_GetItemToUse(On_Player.orig_QuickMount_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item == null && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsConsumeItem) {
                    continue;
                }
                item = BackpackLoader.GetQuickMountItem(player, backpackPlayer.backpacks[i]);
            }
        }

        return item;
    }
}
