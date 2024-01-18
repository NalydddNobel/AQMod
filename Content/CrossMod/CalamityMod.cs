using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod;

internal class CalamityMod : SupportedMod<CalamityMod> {
    public override void SafeLoad(Mod mod) {
        ModCompatabilityFlags.RemoveExpertExclusivity = true;
    }

    public override void PostSetupContent() {
    }
}