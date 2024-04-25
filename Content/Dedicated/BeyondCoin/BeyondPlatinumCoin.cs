using Aequus.Common.Items.Dedications;
using Terraria.ModLoader;

namespace Aequus.Content.Dedicated.BeyondCoin;

public class BeyondPlatinumCoin : ModItem {
    public override void SetDefaults() {
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(platinum: 100);
    }

    public override void Load() {
        DedicationRegistry.Register(this, new AnonymousDedication(new Color(105, 97, 191)));
    }

    public override void SetStaticDefaults() {
        ItemSets.ShimmerTransformToItem[Type] = ModContent.ItemType<ShimmerCoin>();
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.PlatinumCoin, 100)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumCoin);

        Recipe.Create(ItemID.PlatinumCoin, 100)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumCoin);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.5f);
    }
}
