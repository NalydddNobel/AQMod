namespace Aequus.Content.Equipment.Accessories.SpiritBottle;

public class BottleOSpirits : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
    }
}