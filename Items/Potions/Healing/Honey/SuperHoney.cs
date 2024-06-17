namespace Aequus.Items.Potions.Healing.Honey;

public class SuperHoney : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.healLife = 160;
        Item.buffType = BuffID.Honey;
        Item.buffTime = 1800;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Lime;
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
        CreateRecipe(4)
            .AddIngredient<GreaterHoney>(4)
            .AddIngredient(ItemID.FragmentNebula)
            .AddIngredient(ItemID.FragmentSolar)
            .AddIngredient(ItemID.FragmentStardust)
            .AddIngredient(ItemID.FragmentVortex)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.SuperHealingPotion)
            .DisableDecraft();
    }
}
