using Aequus.Content.DataSets;
using Aequus.Core.CrossMod;

namespace Aequus.Content.CrossMod; 

internal class ThoriumMod : SupportedMod<ThoriumMod> {
    public override void SafeLoad(Mod mod) {
    }

    public override void AddRecipes() {
        foreach (int buffId in BuffSets.PlayerDoTDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Aequus.Instance) {
                Call("AddPlayerDoTBuffID", buffId);
            }
        }
        foreach (int buffId in BuffSets.PlayerStatusDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Aequus.Instance) {
                Call("AddPlayerStatusBuffID", buffId);
            }
        }
    }

    public override void SafeUnload() {
    }
}