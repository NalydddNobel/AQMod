namespace Aequus.Content.Potions.Mana;

public class MeadowMushroom : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Mushroom);
        Item.healMana = Item.healLife;
        Item.potion = false;
        Item.healLife = 0;
    }
}
