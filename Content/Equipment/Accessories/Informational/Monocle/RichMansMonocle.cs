namespace Aequus.Content.Equipment.Accessories.Informational.Monocle;

public class RichMansMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
        ItemSets.ShimmerTransformToItem[Type] = ModContent.ItemType<ShimmerMonocle>();
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accMonocle = true;
    }
}