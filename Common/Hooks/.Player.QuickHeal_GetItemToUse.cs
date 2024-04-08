using Aequus.Common.Backpacks;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static Item On_Player_QuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item == null && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsConsumeItem) {
                    continue;
                }
                item = BackpackLoader.GetQuickHealItem(player, backpackPlayer.backpacks[i]);
            }
        }

        return item;
    }
}
