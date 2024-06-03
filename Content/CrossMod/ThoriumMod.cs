using Aequus.Core.CrossMod;
using Aequus.DataSets;

namespace Aequus.Content.CrossMod;

internal class ThoriumMod : SupportedMod<ThoriumMod> {
    public override void AddRecipes() {
        foreach (int buffId in BuffDataSet.PlayerDoTDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Aequus.Instance) {
                Call("AddPlayerDoTBuffID", buffId);
            }
        }
        foreach (int buffId in BuffDataSet.PlayerStatusDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Aequus.Instance) {
                Call("AddPlayerStatusBuffID", buffId);
            }
        }
    }
}