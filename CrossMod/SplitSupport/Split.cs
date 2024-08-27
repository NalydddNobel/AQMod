using Aequus.CrossMod;

namespace Aequus.Content.CrossMod.SplitSupport;
internal partial class Split : ModSupport<Split> {
    public override void PostSetupContent() {
        LoadPhotographySupport();
    }
}