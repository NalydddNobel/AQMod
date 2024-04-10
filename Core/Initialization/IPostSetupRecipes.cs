namespace Aequus.Core.Initialization;

/// <summary>Runs after PostAddRecipes. Do not edit recipes here.</summary>
internal interface IPostSetupRecipes : ILoad {
    void PostSetupRecipes(Aequus aequus);
}
