using Aequu2.Core.CrossMod;
using Aequu2.DataSets;

namespace Aequu2.Content.CrossMod;

internal class ThoriumMod : SupportedMod<ThoriumMod> {
    public override void AddRecipes() {
        foreach (int buffId in BuffDataSet.PlayerDoTDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Aequu2.Instance) {
                Call("AddPlayerDoTBuffID", buffId);
            }
        }
        foreach (int buffId in BuffDataSet.PlayerStatusDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Aequu2.Instance) {
                Call("AddPlayerStatusBuffID", buffId);
            }
        }
    }
}