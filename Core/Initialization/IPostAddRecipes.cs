namespace Aequus.Core.Initialization;

internal interface IPostAddRecipes : ILoadable {
    void PostAddRecipes(Aequus aequus);
}
