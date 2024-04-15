namespace Aequus.Content.Tools.Keys;

public class CopperKey : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.GoldenKey);
        Item.rare = ItemRarityID.White;
    }
}
