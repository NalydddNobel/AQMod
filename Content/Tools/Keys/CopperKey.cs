using Aequus.DataSets;

namespace Aequus.Content.Tools.Keys;

public class CopperKey : ModItem {
    public override void SetStaticDefaults() {
        ItemDataSet.KeychainData[Type] = new(NonConsumable: false);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.GoldenKey);
        Item.rare = ItemRarityID.White;
    }
}
