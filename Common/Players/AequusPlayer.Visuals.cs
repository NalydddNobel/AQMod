using Aequus.Common.Players.Attributes;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool visualAfterImages;

    public override void FrameEffects() {
        if (visualAfterImages) {
            Player.armorEffectDrawShadow = true;
        }
    }
}