using AequusRemake.Systems.Backpacks;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for backpacks to provide healing items. Backpack healing potions do not calculate with the life restoration bias used for Restoration potions.</summary>
    private static Item On_Player_QuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player player) {
        Item item = orig(player);

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
