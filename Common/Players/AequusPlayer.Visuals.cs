using Aequus.Common.Players.Attributes;
using Aequus.Common.Players.Dashes;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool visualAfterImages;

    private void Load_Visuals() {
        On_Player.UpdateVisibleAccessories += On_Player_UpdateVisibleAccessories;
    }

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

    public override void FrameEffects() {
        if (visualAfterImages) {
            Player.armorEffectDrawShadow = true;
        }
        if (Player.dashDelay != 0 && Player.dash == -1 && DashData != null) {
            DashData.OnPlayerFrame(Player, this);
        }
    }
}