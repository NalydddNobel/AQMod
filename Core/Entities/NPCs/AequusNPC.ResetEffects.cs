using AequusRemake.Core.CodeGeneration;

namespace AequusRemake.Core.Entities.NPCs;

public partial class AequusRemakeNPC {
    private static ResetEffectsGenerator<AequusRemakeNPC> _resetEffects;

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