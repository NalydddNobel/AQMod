using Aequus.Core.Entities.Players;

namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for custom dash accessories added by Aequus to update visual effects.</summary>
    private static void On_Player_UpdateVisibleAccessories(On_Player.orig_UpdateVisibleAccessories orig, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            orig(player);
            return;
        }

        CustomDashData dashData = player.dashDelay < 0 && player.dash == -1 && aequusPlayer.DashData != null ? aequusPlayer.DashData : null;
        dashData?.PreUpdateVisibleAccessories(player, aequusPlayer);
        orig(player);
        dashData?.PostUpdateVisibleAccessories(player, aequusPlayer);
    }
}
