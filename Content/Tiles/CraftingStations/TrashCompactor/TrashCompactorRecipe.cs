using Aequus.Content.DataSets;
using System.Collections.Generic;
using System.Linq;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public struct TrashCompactorRecipe {
    public static readonly TrashCompactorRecipe None = default(TrashCompactorRecipe);

    public TrashCompactorRecipe(int ingredient, params (int, int)[] results) : this(new Item(ingredient), results.Select((i) => new Item(i.Item1, i.Item2)).ToList()) {
    }

    public TrashCompactorRecipe(int ingredient, params Item[] results) : this(new Item(ingredient), results.ToList()) {
    }

    public TrashCompactorRecipe(Recipe recipe) : this(recipe.createItem, recipe.requiredItem) {
    }

    public TrashCompactorRecipe(Item ingredient, List<Item> results) {
        Results = results;
        Ingredient = ingredient;
    }

    public readonly List<Item> Results;
    public readonly Item Ingredient;

    public bool Invalid => Ingredient == null || Results == null || Results.Count <= 0;

    public static TrashCompactorRecipe FromItem(Item item) {
        if (ItemSets.CustomTrashCompactorRecipes.TryGetValue(item.type, out var recipeOverride)) {
            return recipeOverride;
        }

        // Cannot get recipe if the item is air or cannot be shimmered.
        if (item.IsAir || !item.CanShimmer()) {
            return None;
        }

        // Only get recipes for items which can place a "Frame Important" (non-block) tile, and isn't a generic torch.
        if (item.createTile > -1 && Main.tileFrameImportant[item.createTile] && !TileID.Sets.Torch[item.createTile]) {
            //if (true) {

            if (item.createTile > -1) {
                // Prevent decrafting for 1x1 tiles which are not light sources like Candles. (Bars, misc)
                var tileObjectData = TileObjectData.GetTileData(item.createTile, item.placeStyle);
                if (tileObjectData != null && tileObjectData.Width == 1 && tileObjectData.Height == 1 && !TileID.Sets.RoomNeeds.CountsAsTorch.Any(item.createTile)) {
                    return None;
                }
            }

            var resultList = None;
            for (int i = 0; i < Recipe.numRecipes; i++) {
                if (Main.recipe[i] == null || Main.recipe[i].createItem.type != item.type || !Main.recipe[i].NotDisabledAndConditionsMet()) {
                    continue;
                }

                // Item has multiple recipes, cannot decraft!
                if (!resultList.Invalid) {
                    return None;
                }

                resultList = new(Main.recipe[i]);
            }

            return resultList;
        }

        return None;
    }

    public int GetIngredientQuantity(Item ingredient) {
        return ingredient.stack / Ingredient.stack;
    }

    public static void AddCustomRecipe(int ingredient, params (int, int)[] results) {
        ItemSets.CustomTrashCompactorRecipes[ingredient] = new(ingredient, results);
    }
}