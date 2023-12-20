using System.Collections.Generic;

namespace Aequus.Core;
public interface IPoolable {
    bool Resting { get; set; }
}

public static class PoolableExtensions {
    public static void Rest<T>(this T poolable) where T : IPoolable, new() {
        InstancePool<T>.Rest(poolable);
    }
}

public static class InstancePool<T> where T : IPoolable, new() {
    public static List<T> RestingPool { get; private set; } = new(8);

    public static T Get() {
        lock (RestingPool) {
            if (RestingPool.Count > 0) {
                for (int i = 0; i < RestingPool.Count; i++) {
                    var t = RestingPool[i];
                    if (!t.Resting) {
                        continue;
                    }

                    t.Resting = false;
                    RestingPool.RemoveAt(i);
                    return t;
                }
            }

            var newInstance = new T();
            RestingPool.Add(newInstance);
            return newInstance;
        }
    }

    internal static void Rest(T poolable) {
        poolable.Resting = true;
        lock (RestingPool) {
            RestingPool.Add(poolable);
        }
    }
}