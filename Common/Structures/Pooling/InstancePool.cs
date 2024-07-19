using System.Collections.Generic;

namespace Aequus.Common.Structures.Pooling;

public static class InstancePool<T> where T : IPoolable, new() {
    private static readonly Stack<T> _pool = new(8);

    public static T Get() {
        lock (_pool) {
            if (_pool.Count == 0) {
                _pool.Push(new T());
            }
            var instance = _pool.Pop();
            instance.Resting = false;
            return instance;
        }
    }

    internal static void Rest(T poolable) {
        poolable.Resting = true;
        lock (_pool) {
            _pool.Push(poolable);
        }
    }
}
