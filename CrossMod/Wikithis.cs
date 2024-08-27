namespace Aequus.CrossMod;
internal class Wikithis : ModSupport<Wikithis> {
    public override void SafeLoad(Mod mod) {
        if (!Main.dedServ) {
            mod.Call("AddModURL", Mod, "https://terrariamods.wiki.gg/wiki/Aequus/{}");
        }
    }
}