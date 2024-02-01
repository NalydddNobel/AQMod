using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod;

internal class Wikithis : SupportedMod<Wikithis> {
    public override void PostSetupContent() {
        Instance.Call("AddModURL", Mod, "https://terrariamods.wiki.gg/wiki/Aequus/{}");
    }
}