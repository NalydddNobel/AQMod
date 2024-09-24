namespace Aequus.Common;

public class LoadedType : ILoadable, IPostSetupContent, IAddRecipes {
    private Mod? _mod;
    protected Mod Mod => _mod!;

    protected virtual void Load() { }
    protected virtual void PostSetupContent() { }
    protected virtual void AddRecipes() { }
    protected virtual void Unload() { }
    protected virtual bool IsLoadingEnabled(Mod mod) {
        return true;
    }

    bool ILoadable.IsLoadingEnabled(Mod mod) {
        return IsLoadingEnabled(mod);
    }

    void ILoadable.Load(Mod mod) {
        _mod = mod;
        Load();
    }

    void IPostSetupContent.PostSetupContent() {
        PostSetupContent();
    }

    void IAddRecipes.AddRecipes() {
        AddRecipes();
    }

    void ILoadable.Unload() {
        Unload();
    }
}
