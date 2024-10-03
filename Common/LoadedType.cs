namespace Aequus.Common;

public class LoadedType : ILoadable, IPostSetupContent, IAddRecipes, IPostAddRecipes {
    private Mod? _mod;
    protected Mod Mod => _mod!;

    protected virtual void Load() { }
    public virtual void PostSetupContent() { }
    public virtual void AddRecipes() { }
    public virtual void PostAddRecipes() { }
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

    void ILoadable.Unload() {
        Unload();
    }
}
