namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows infinite wormhole usage for the Phase Mirror. (<see cref="AequusPlayer.infiniteWormhole"/>)</summary>
    private static bool Player_HasUnityPotion(On_Player.orig_HasUnityPotion orig, Player player) {
        return player.GetModPlayer<AequusPlayer>().infiniteWormhole ? true : orig(player);
    }
}
