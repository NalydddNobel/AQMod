using Aequus.Common.Buffs;
using Aequus.Common.CrossMod;
using Aequus.Common.DataSets;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.CrossMod.ThoriumModSupport {
    internal class ThoriumMod : ModSupport<ThoriumMod> {

        public override void SafeLoad(Mod mod) {
        }

        public override void AddRecipes() {
            foreach (var b in BuffSets.PlayerDoTDebuff) {
                if (b >= BuffID.Count && BuffLoader.GetBuff(b).Mod.Name == "Aequus") {
                    Call("AddPlayerDoTBuffID", b);
                }
            }
            foreach (var b in BuffSets.PlayerStatusDebuff) {
                if (b >= BuffID.Count && BuffLoader.GetBuff(b).Mod.Name == "Aequus") {
                    Call("AddPlayerStatusBuffID", b);
                }
            }
        }

        public override void SafeUnload() {
        }
    }
}