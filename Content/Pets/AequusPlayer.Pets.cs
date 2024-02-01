using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool petLanternLuckCat;
    [ResetEffects]
    public bool petOmegaStarite;
    [ResetEffects]
    public bool petSwagEye;
    [ResetEffects]
    public bool petFamiliar;
    [ResetEffects]
    public bool petUndeadMiner;

    private void ModifyLuck_LanternCat(ref float luck) {
        if (petLanternLuckCat) {
            luck += 0.05f;
        }
    }
}