namespace Aequus.Common.Structures.Pooling;

public static class PoolableExtensions {
    public static void Rest<T>(this T poolable) where T : IPoolable, new() {
        InstancePool<T>.Rest(poolable);
    }
}