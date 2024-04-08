namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static bool Player_HasUnityPotion(On_Player.orig_HasUnityPotion orig, Player player) {
        return player.GetModPlayer<AequusPlayer>().infiniteWormhole ? true : orig(player);
    }
}
