using AequusRemake.Core.CrossMod;
using AequusRemake.DataSets;

namespace AequusRemake.Content.CrossMod;

internal class ThoriumMod : SupportedMod<ThoriumMod> {
    public override void AddRecipes() {
        foreach (int buffId in BuffDataSet.PlayerDoTDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == AequusRemake.Instance) {
                Call("AddPlayerDoTBuffID", buffId);
            }
        }
        foreach (int buffId in BuffDataSet.PlayerStatusDebuff) {
            if (buffId >= BuffID.Count && BuffLoader.GetBuff(buffId)?.Mod == AequusRemake.Instance) {
                Call("AddPlayerStatusBuffID", buffId);
            }
        }
    }
}