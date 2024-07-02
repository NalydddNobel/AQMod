using Aequus.Core.CodeGeneration;

namespace Aequus.Core.Entities.NPCs;

public partial class AequusNPC {
    private static ResetEffectsGenerator<AequusNPC> _resetEffects;

    private void Load_AutomaticResetEffects() {
        _resetEffects = new();
        _resetEffects.Generate();
    }

    public override void ResetEffects(NPC npc) {
        if (immuneToDamageTime > 0) {
            immuneToDamageTime--;
        }
        _resetEffects.Invoke(this);
    }
}