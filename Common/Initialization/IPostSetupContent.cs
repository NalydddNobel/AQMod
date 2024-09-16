namespace Aequus;

/// <summary>Runs after <see cref="IOnModLoad.OnModLoad(Aequus)"/>.</summary>
internal interface IPostSetupContent : ILoadable {
    void PostSetupContent();
}
