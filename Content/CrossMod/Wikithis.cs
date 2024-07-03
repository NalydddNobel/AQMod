using Aequu2.Core.CrossMod;

namespace Aequu2.Content.CrossMod;

internal class Wikithis : SupportedMod<Wikithis> {
    public override void PostSetupContent() {
        Instance.Call("AddModURL", Mod, "https://terrariamods.wiki.gg/wiki/Aequu2/{}");
    }
}