namespace Aequus;

public partial class AequusPlayer : ModPlayer {
    public override void ModifyLuck(ref float luck) {
        ModifyLuck_LanternCat(ref luck);
    }
}