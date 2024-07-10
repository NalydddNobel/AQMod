namespace AequusRemake.Systems.CrossMod;

internal class Wikithis : SupportedMod<Wikithis> {
    public override void PostSetupContent() {
        Instance.Call("AddModURL", Mod, "https://terrariamods.wiki.gg/wiki/AequusRemake/{}");
    }
}