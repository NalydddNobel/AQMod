namespace Aequus.Content.Potions.Mana;

public class MeadowMushroom : ModItem {
    public override void SetStaticDefaults() {
        Item.CloneResearchCount(ItemID.Mushroom);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Mushroom);
        Item.healMana = Item.healLife;
        Item.potion = false;
        Item.healLife = 0;
    }

    public override void AddRecipes() {
        Recipe.Create(ItemID.LesserManaPotion, 2)
            .AddIngredient(Type)
            .AddIngredient(ItemID.Gel, 2)
            .AddIngredient(ItemID.Bottle, 2)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.ManaPotion);
    }
}
