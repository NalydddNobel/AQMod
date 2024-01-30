using Aequus.Core.Generator;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public System.Boolean petLanternLuckCat;
    [ResetEffects]
    public System.Boolean petOmegaStarite;
    [ResetEffects]
    public System.Boolean petSwagEye;
    [ResetEffects]
    public System.Boolean petFamiliar;
    [ResetEffects]
    public System.Boolean petUndeadMiner;

    private void ModifyLuck_LanternCat(ref System.Single luck) {
        if (petLanternLuckCat) {
            luck += 0.05f;
        }
    }
}