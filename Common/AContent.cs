namespace Aequus.Common;

public class AContent : ILoadable {
    protected Mod? Mod { get; private set; }

    protected virtual void Load() { }
    protected virtual void Unload() { }


    void ILoadable.Load(Mod mod) {
        Mod = mod;
        Load();
    }

    void ILoadable.Unload() {
        Unload();
        Mod = null;
    }
}
