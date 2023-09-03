using Aequus.Common.Players.Attributes;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool visualAfterImages;

    public override void FrameEffects() {
        if (visualAfterImages) {
            Player.armorEffectDrawShadow = true;
        }
        if (Player.dashDelay > 0 && Player.dash == -1 && DashData != null) {
            DashData.OnPlayerFrame(Player, this);
        }
    }
}