namespace Aequus;

/// <summary>Runs after <see cref="IPostSetupContent.PostSetupContent()"/>.</summary>
internal interface IAddRecipes : ILoadable {
    void AddRecipes();
}
