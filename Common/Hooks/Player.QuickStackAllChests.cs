using Aequus.Common.Backpacks;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for backpacks to quick stack their items to nearby chests.</summary>
    private static void On_Player_QuickStackAllChests(On_Player.orig_QuickStackAllChests orig, Player player) {
        orig(player);
        if (player.HasLockedInventory() || !player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }

        for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
            if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsQuickStack) {
                continue;
            }
            BackpackLoader.QuickStackToNearbyChest(player.Center, backpackPlayer.backpacks[i]);
        }
    }
}
