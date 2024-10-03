using Aequus.Common;
using Aequus.Content.Systems.Keys.Keychains;

namespace Aequus.Content.Systems.Keys;

public class KeyHooks : LoadedType {
    protected override void Load() {
        On_Player.ConsumeItem += On_Player_ConsumeItem;
    }

    private static bool On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player player, int type, bool reverseOrder, bool includeVoidBag) {
        // Check Keychain for consuming an item.
        if (player.TryGetModPlayer(out KeychainPlayer keychain) && keychain.ConsumeKey(player, type)) {
            keychain.RefreshKeys();
            return true;
        }

        return orig(player, type, reverseOrder, includeVoidBag);
    }
}
