using Aequus.Common.CrossMod;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.CrossMod {
    internal class Wikithis : ModSupport<Wikithis> {
        public override void SafeLoad(Mod mod) {
            if (mod != null && !Main.dedServ) {
                mod.Call("AddModURL", Mod, "terrariamods.wiki.gg$Aequus");
            }
        }
    }
}