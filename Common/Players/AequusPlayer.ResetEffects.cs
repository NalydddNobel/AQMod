using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    private static ResetEffectsGenerator<AequusPlayer> _resetEffects;

    public override void ResetEffects() {
#if !DEBUG
        ResetEffects_LegacyNecromancy();
#endif

        ResetEffectsInner();
        if (Player.dashDelay == 0) {
            DashData = null;
        }
        _resetEffects.Invoke(this);
    }
}