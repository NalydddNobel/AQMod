namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Player_ItemCheck_ReleaseCritter(On_Player.orig_ItemCheck_ReleaseCritter orig, Player player, Item sItem) {
        if (player.TryGetModPlayer(out AequusPlayer aequus) && aequus.disableItem > 0) {
            return;
        }

        orig(player, sItem);
    }
}
