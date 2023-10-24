using Aequus.Core.Generator;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    private static ResetEffectsGenerator<AequusPlayer> _resetEffects;

    public override void ResetEffects() {
        if (Player.dashDelay == 0) {
            DashData = null;
        }
        _resetEffects.Invoke(this);
    }
}