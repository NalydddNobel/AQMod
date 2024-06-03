namespace Aequus;

public partial class AequusPlayer {
    public override void ModifyLuck(ref float luck) {
        ModifyLuck_LanternCat(ref luck);
    }
}