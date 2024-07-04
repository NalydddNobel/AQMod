using AequusRemake.Core.CodeGeneration;

namespace AequusRemake;

public partial class AequusPlayer {
    private static ResetEffectsGenerator<AequusPlayer> _resetEffects;

    public override void ResetEffects() {
        ResetEffectsInner();
        if (Player.dashDelay == 0) {
            DashData = null;
        }
        _resetEffects.Invoke(this);
    }
}