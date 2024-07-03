using Aequu2.Core.CodeGeneration;

namespace Aequu2;

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