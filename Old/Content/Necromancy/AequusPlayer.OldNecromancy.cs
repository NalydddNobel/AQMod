using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public int ghostShadowDash;
    [ResetEffects]
    public float zombieDebuffMultiplier;
    [ResetEffects]
    public int ghostProjExtraUpdates;
    [ResetEffects]
    public bool accRitualSkull;
    [ResetEffects]
    public int ghostChains;
    [ResetEffects]
    public int ghostSlots;
    public int ghostSlotsOld;
    [ResetEffects(2)]
    public int ghostSlotsMax;
    [ResetEffects(3600)]
    public int ghostLifespan;

    [ResetEffects(-1)]
    public int gravetenderGhost;

    private void ResetEffects_LegacyNecromancy() {
        ghostSlotsOld = ghostSlots;
    }

    private void UpdateLegacyNecromancyAccs() {
        if (accRitualSkull) {
            ghostSlots += Player.maxMinions;
            Player.maxMinions = 1;
        }
    }
}
