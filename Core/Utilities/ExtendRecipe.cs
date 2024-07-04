using System;

namespace AequusRemake.Core.Utilities;

public static class ExtendRecipe {
    /// <summary></summary>
    /// <param name="action">Return <see langword="false"/> to stop looping over recipes.</param>
    public static void ForEachRecipe(Func<Recipe, bool> action) {
        for (int i = 0; i < Recipe.numRecipes; i++) {
            if (Main.recipe[i]?.createItem?.IsAir == false) {
                if (!action(Main.recipe[i])) {
                    return;
                }
            }
        }
    }

    /// <returns><see langword="true"/>, if the recipe is not disabled, and conditions have been met. Otherwise <see langword="false"/>.</returns>
    public static bool NotDisabledAndConditionsMet(this Recipe recipe) {
        if (recipe.Disabled) {
            return false;
        }

        foreach (var condition in recipe.DecraftConditions) {
            if (!condition.IsMet()) {
                return false;
            }
        }

        return true;
    }

    /// <summary>Finds an ingredient of a specified type in the recipe.</summary>
    /// <param name="recipe"></param>
    /// <param name="itemID">The Item Id to search for.</param>
    /// <returns>An Item instance if it was found in the recipe. Otherwise <see langword="null"/>.</returns>
    public static Item FindIngredient(this Recipe recipe, int itemID) {
        return recipe.requiredItem.Find((item) => item != null && !item.IsAir && item.type == itemID);
    }
    /// <summary><inheritdoc cref="FindIngredient(Recipe, int)"/></summary>
    /// <param name="recipe"></param>
    /// <param name="itemID">The Item Id to search for.</param>
    /// <param name="result">The Item instance.</param>
    /// <returns><see langword="true"/>, if the Item was found in the recipe. Otherwise <see langword="false"/>.</returns>
    public static bool TryFindIngredient(this Recipe recipe, int itemID, out Item result) {
        result = recipe.FindIngredient(itemID);
        return result != null;
    }

    /// <summary>Replaces an item of a specified id with another item of a specified id.</summary>
    /// <param name="recipe"></param>
    /// <param name="oldItem">The Item Id to be replaced by <paramref name="newItem"/>.</param>
    /// <param name="newItem">The Item Id to replace <paramref name="oldItem"/> with.</param>
    /// <param name="newItemStack">Stack override for <paramref name="newItem"/>. A value of -1 will keep the stack of <paramref name="oldItem"/></param>
    public static Recipe ReplaceItem(this Recipe recipe, int oldItem, int newItem, int newItemStack = -1) {
        for (int i = 0; i < recipe.requiredItem.Count; i++) {
            if (recipe.requiredItem[i].type == oldItem) {
                int stack = newItemStack <= 0 ? recipe.requiredItem[i].stack : newItemStack;
                recipe.requiredItem[i].SetDefaults(newItem);
                recipe.requiredItem[i].stack = stack;
                break;
            }
        }
        return recipe;
    }
}
