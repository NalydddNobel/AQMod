using Aequus.Common.Backpacks;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows items in backpacks to be consumed.</summary>
    private static bool On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player player, int type, bool reverseOrder, bool includeVoidBag) {
        bool consumedItem = orig(player, type, reverseOrder, includeVoidBag);

        if (!consumedItem && includeVoidBag && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsConsumeItem) {
                    continue;
                }
                BackpackLoader.ConsumeItem(player, backpackPlayer.backpacks[i], type, reverseOrder);
            }
        }

        return consumedItem;
    }
}
