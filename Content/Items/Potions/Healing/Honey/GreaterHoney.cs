namespace Aequus.Content.Items.Potions.Healing.Honey;

public class GreaterHoney : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.buffType = BuffID.Honey;
        Item.buffTime = 1500;
        Item.healLife = 120;
        Item.value = Item.sellPrice(silver: 20);
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.Item3;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 14;
        Item.height = 24;
        Item.potion = true;
    }

    public override void AddRecipes() {
        CreateRecipe(3)
            .AddIngredient(ItemID.BottledHoney, 3)
            .AddIngredient(ItemID.PixieDust, 3)
            .AddIngredient(ItemID.CrystalShard)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.GreaterHealingPotion)
            .DisableDecraft();
    }
}
