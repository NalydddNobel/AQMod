namespace Aequus.Items.Equipment.Vanity;
public class EyeGlint : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 20);
        Item.vanity = true;
        Item.hasVanityEffects = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateEquip(Player player) {
        player.Aequus().eyeGlint = true;
    }

    public override void UpdateVanity(Player player) {
        player.Aequus().eyeGlint = true;
    }
}