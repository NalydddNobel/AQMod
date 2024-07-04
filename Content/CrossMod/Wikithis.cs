using AequusRemake.Core.CrossMod;

namespace AequusRemake.Content.CrossMod;

internal class Wikithis : SupportedMod<Wikithis> {
    public override void PostSetupContent() {
        Instance.Call("AddModURL", Mod, "https://terrariamods.wiki.gg/wiki/AequusRemake/{}");
    }
}