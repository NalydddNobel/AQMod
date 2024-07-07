using System;

namespace AequusRemake.Core.Util.Extensions;

public sealed class RecipeTools {
    /// <summary>Safely iterates over all valid recipes.</summary>
    /// <param name="action">Return <see langword="false"/> to stop looping over recipes.</param>
    public static void ForEach(Func<Recipe, bool> action) {
        for (int i = 0; i < Recipe.numRecipes; i++) {
            Recipe recipe = Main.recipe[i];

            if (recipe == null || recipe.Disabled || recipe.createItem == null || recipe.createItem.IsAir) {
                continue;
            }

            if (!action(Main.recipe[i])) {
                return;
            }
        }
    }
}
