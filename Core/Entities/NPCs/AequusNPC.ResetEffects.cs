using Aequu2.Core.CodeGeneration;

namespace Aequu2.Core.Entities.NPCs;

public partial class Aequu2NPC {
    private static ResetEffectsGenerator<Aequu2NPC> _resetEffects;

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