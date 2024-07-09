using Terraria.DataStructures;

namespace AequusRemake.Systems.Items;

internal interface IOnConsumedInRecipe {
    void OnConsumedInRecipe(Item createdItem, RecipeItemCreationContext context);
}

internal sealed class OnConsumedInRecipeGlobalItem : GlobalItem {
    public override void OnCreated(Item item, ItemCreationContext context) {
        if (context is RecipeItemCreationContext recipeContext) {
            foreach (var consumedItem in recipeContext.ConsumedItems) {
                if (consumedItem.ModItem is IOnConsumedInRecipe onConsumed) {
                    onConsumed.OnConsumedInRecipe(consumedItem, recipeContext);
                }
            }
        }
    }
}