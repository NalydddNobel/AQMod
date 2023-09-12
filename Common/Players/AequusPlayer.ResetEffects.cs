using Aequus.Core.Generator;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    private static ResetEffectsGenerator<AequusPlayer> _resetEffects;

    private void Load_AutomaticResetEffects() {
        _resetEffects = new();
        _resetEffects.Generate();
    }

    public override void ResetEffects() {
        if (Player.dashDelay == 0) {
            DashData = null;
        }
        _resetEffects.Invoke(this);
    }
}