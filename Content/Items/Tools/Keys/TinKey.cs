using AequusRemake.Core;
using AequusRemake.DataSets;

namespace AequusRemake.Content.Items.Tools.Keys;

public class TinKey : ModItem {
    public override void SetStaticDefaults() {
        ItemDataSet.KeychainData[Type] = new(NonConsumable: false, Color: new Color(183, 88, 25));
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.GoldenKey);
        Item.rare = Commons.Rare.BossSalamancer;
    }
}
