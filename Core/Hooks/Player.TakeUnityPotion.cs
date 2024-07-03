namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Prevents the player from consuming a wormhole potion when having the Phase Mirror. (<see cref="AequusPlayer.infiniteWormhole"/>)</summary>
    private static void On_Player_TakeUnityPotion(On_Player.orig_TakeUnityPotion orig, Player player) {
        if (player.GetModPlayer<AequusPlayer>().infiniteWormhole) {
            return;
        }

        orig(player);
    }
}
