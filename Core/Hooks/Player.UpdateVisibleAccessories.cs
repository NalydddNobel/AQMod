using AequusRemake.Core.Entities.Players;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for custom dash accessories added by AequusRemake to update visual effects.</summary>
    private static void On_Player_UpdateVisibleAccessories(On_Player.orig_UpdateVisibleAccessories orig, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var AequusRemakePlayer)) {
            orig(player);
            return;
        }

        CustomDashData dashData = player.dashDelay < 0 && player.dash == -1 && AequusRemakePlayer.DashData != null ? AequusRemakePlayer.DashData : null;
        dashData?.PreUpdateVisibleAccessories(player, AequusRemakePlayer);
        orig(player);
        dashData?.PostUpdateVisibleAccessories(player, AequusRemakePlayer);
    }
}
