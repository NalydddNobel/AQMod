using Aequus.Core.CodeGeneration;

namespace Aequus.Common.NPCs;

public partial class AequusNPC {
    private static ResetEffectsGenerator<AequusNPC> _resetEffects;

    private void Load_AutomaticResetEffects() {
        _resetEffects = new();
        _resetEffects.Generate();
    }

    public override void ResetEffects(NPC npc) {
        _resetEffects.Invoke(this);
    }
}