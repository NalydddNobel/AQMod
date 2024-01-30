namespace Aequus;

public partial class AequusPlayer : ModPlayer {
    public override void ModifyLuck(ref System.Single luck) {
        ModifyLuck_LanternCat(ref luck);
    }
}