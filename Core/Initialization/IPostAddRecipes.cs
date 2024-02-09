namespace Aequus.Core.Initialization;

internal interface IPostAddRecipes : ILoad {
    void PostAddRecipes(Aequus aequus);
}
