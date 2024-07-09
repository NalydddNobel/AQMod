using AequusRemake.Core.CrossMod;
using AequusRemake.DataSets;

namespace AequusRemake.Content.CrossMod;

internal class ThoriumMod : SupportedMod<ThoriumMod> {
    public override void AddRecipes() {
        foreach (int buffId in BuffDataSet.PlayerDoTDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Mod) {
                Call("AddPlayerDoTBuffID", buffId);
            }
        }
        foreach (int buffId in BuffDataSet.PlayerStatusDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == Mod) {
                Call("AddPlayerStatusBuffID", buffId);
            }
        }
    }
}