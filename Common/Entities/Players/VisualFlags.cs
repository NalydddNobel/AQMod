namespace Aequus.Common.Entities.Players;

public class VisualFlags : ModPlayer {
    public bool drawShadow;

    public override void ResetEffects() {
        drawShadow = false;
    }

    public override void FrameEffects() {
        if (drawShadow) {
            Player.armorEffectDrawShadow = true;
        }
    }
}
