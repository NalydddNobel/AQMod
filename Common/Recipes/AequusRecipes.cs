using Aequus.Core.Initialization;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.Items;

public sealed partial class AequusRecipes : ModSystem {
    public static readonly HashSet<int> ItemIdHasPrefixRecipe = new();
    public static readonly Dictionary<int, Condition> ShimmerTransformLocks = new();

    public override void PostAddRecipes() {
        List<IRecipeEditor> editors = Mod.GetContent<IRecipeEditor>().ToList();
        for (int i = 0; i < Recipe.maxRecipes; i++) {
            Recipe recipe = Main.recipe[i];
            if (recipe == null || recipe.createItem == null || recipe.requiredItem == null || recipe.requiredItem.Count == 0) {
                continue;
            }

            for (int k = 0; k < editors.Count; k++) {
                editors[k].EditRecipe(recipe);
            }
        }
    }

    public override void PostSetupRecipes() {
        List<IRecipeScanner> scanners = Mod.GetContent<IRecipeScanner>().ToList();
        for (int i = 0; i < Recipe.maxRecipes; i++) {
            Recipe recipe = Main.recipe[i];
            if (recipe == null || recipe.createItem == null || recipe.requiredItem == null || recipe.requiredItem.Count == 0) {
                continue;
            }

            for (int k = 0; k < scanners.Count; k++) {
                scanners[k].ScanRecipe(recipe);
            }
        }
    }

    public override void Unload() {
        ItemIdHasPrefixRecipe.Clear();
        ShimmerTransformLocks.Clear();
    }
}