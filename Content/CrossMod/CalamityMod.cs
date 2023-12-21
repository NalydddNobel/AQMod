using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod;

internal class CalamityMod : SupportedMod<CalamityMod> {
    public override void SafeLoad(Mod mod) {
        ModCommons.ExpertDropsInClassicMode = true;
    }

    public override void PostSetupContent() {
    }
}