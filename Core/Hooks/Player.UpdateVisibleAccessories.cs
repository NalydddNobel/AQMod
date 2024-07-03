using Aequu2.Core.Entities.Players;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for custom dash accessories added by Aequu2 to update visual effects.</summary>
    private static void On_Player_UpdateVisibleAccessories(On_Player.orig_UpdateVisibleAccessories orig, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var Aequu2Player)) {
            orig(player);
            return;
        }

        CustomDashData dashData = player.dashDelay < 0 && player.dash == -1 && Aequu2Player.DashData != null ? Aequu2Player.DashData : null;
        dashData?.PreUpdateVisibleAccessories(player, Aequu2Player);
        orig(player);
        dashData?.PostUpdateVisibleAccessories(player, Aequu2Player);
    }
}
