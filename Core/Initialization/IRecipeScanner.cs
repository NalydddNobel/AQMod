namespace Aequus.Core.Initialization;

internal interface IRecipeScanner : ILoad {
    void ScanRecipe(Recipe recipe);
}
