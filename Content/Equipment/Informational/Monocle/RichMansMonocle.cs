namespace Aequus.Content.Equipment.Informational.Monocle;

public class RichMansMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoMoneyMonocle = true;
    }
}