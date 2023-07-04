namespace Aequus; 

public partial class AequusPlayer {
    public int accWarHorn;
    public bool cooldownWarHorn;

    private void ResetEffects_WarHorn() {
        accWarHorn = 0;
        cooldownWarHorn = false;
    }
}