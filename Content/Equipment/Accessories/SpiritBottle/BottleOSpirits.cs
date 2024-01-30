namespace Aequus.Content.Equipment.Accessories.SpiritBottle;

// TODO -- Actually implement this item.
[LegacyName("Malediction")]
public class BottleOSpirits : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
    }
}