using Aequus.Common.CrossMod;

namespace Aequus.CrossMod;
internal class Wikithis : ModSupport<Wikithis> {
    public override void SafeLoad(Mod mod) {
        if (mod != null && !Main.dedServ) {
            mod.Call("AddModURL", this, "https://examplemod.wiki.gg/wiki/{}");
        }
    }
}