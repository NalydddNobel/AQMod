namespace Aequus;

public partial class AequusPlayer {
    public int ghostShadowDash;
    public float zombieDebuffMultiplier;
    public int ghostProjExtraUpdates;
    public bool accRitualSkull;
    public int ghostChains;
    public int ghostSlots;
    public int ghostSlotsOld;
    public int ghostSlotsMax;
    public int ghostLifespan;

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
