using Aequus.Content.DataSets;
using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod; 

internal class ThoriumMod : SupportedMod<ThoriumMod> {
    public override void SafeLoad(Mod mod) {
    }

    public override void AddRecipes() {
        foreach (var b in BuffSets.PlayerDoTDebuff) {
            if (b >= BuffID.Count && BuffLoader.GetBuff(b).Mod == Aequus.Instance) {
                Call("AddPlayerDoTBuffID", b);
            }
        }
        foreach (var b in BuffSets.PlayerStatusDebuff) {
            if (b >= BuffID.Count && BuffLoader.GetBuff(b).Mod == Aequus.Instance) {
                Call("AddPlayerStatusBuffID", b);
            }
        }
    }

    public override void SafeUnload() {
    }
}