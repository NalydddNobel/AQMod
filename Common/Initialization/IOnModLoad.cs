namespace Aequus;

/// <summary>Runs after <see cref="ILoadable.Load(Mod)"/>.</summary>
internal interface IOnModLoad : ILoadable {
    /// <summary><inheritdoc cref="IOnModLoad"/></summary>
    void OnModLoad();
}
