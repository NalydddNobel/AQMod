using System;

namespace Aequus.Common.Utilities;

internal class CacheProp<T>(Func<T> get, Action<T> set) {
    private bool _hasOldValue;
    private T _old;

    public CacheProp(RefFunc<T> reference) : this(() => reference(), (t) => reference() = t) { }

    public void Overwrite(T value) {
        if (!_hasOldValue) {
            _old = get();
            _hasOldValue = true;
        }
        set(value);
    }

    public void UndoOverwrite() {
        if (_hasOldValue) {
            set(_old);
            _hasOldValue = false;
        }
    }
}

internal class CacheProps : LoadedType {
    public readonly CacheProp<bool> BloodMoon = new(() => ref Main.bloodMoon);
    public readonly CacheProp<bool> Eclipse = new(() => ref Main.eclipse);
    public readonly CacheProp<bool> PumpkinMoon = new(() => ref Main.pumpkinMoon);
    public readonly CacheProp<bool> FrostMoon = new(() => ref Main.snowMoon);
}
