namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_Player_TakeUnityPotion(On_Player.orig_TakeUnityPotion orig, Player player) {
        if (player.GetModPlayer<AequusPlayer>().infiniteWormhole) {
            return;
        }

        orig(player);
    }
}
