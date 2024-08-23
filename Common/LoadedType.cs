namespace Aequus.Common;

public class LoadedType : ILoadable {
    protected Mod? Mod { get; private set; }

    protected virtual void Load() { }
    protected virtual void Unload() { }
    protected virtual bool IsLoadingEnabled(Mod mod) {
        return true;
    }

    bool ILoadable.IsLoadingEnabled(Mod mod) {
        return IsLoadingEnabled(mod);
    }

    void ILoadable.Load(Mod mod) {
        Mod = mod;
        Load();
    }

    void ILoadable.Unload() {
        Unload();
        Mod = null;
    }
}
