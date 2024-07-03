using Aequu2.Content.Backpacks;
using Aequu2.Content.Items.Tools.Keychain;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows items in backpacks to be consumed.</summary>
    private static bool On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player player, int type, bool reverseOrder, bool includeVoidBag) {
        bool consumedItem = orig(player, type, reverseOrder, includeVoidBag);

        if (consumedItem) {
            return true;
        }

        // Check Backpacks for consuming an item.
        if (includeVoidBag && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (backpackPlayer.backpacks[i].IsActive(player) && backpackPlayer.backpacks[i].SupportsConsumeItem && BackpackLoader.ConsumeItem(player, backpackPlayer.backpacks[i], type, reverseOrder)) {
                    return true;
                }
            }
        }

        // Check Keychain for consuming an item.
        if (player.TryGetModPlayer(out KeychainPlayer keychain) && keychain.ConsumeKey(player, type) == true) {
            keychain.RefreshKeys();
            return true;
        }

        return false;
    }
}
