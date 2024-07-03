namespace Aequu2.Core.Utilities;

public static class XPoolable {
    public static void Rest<T>(this T poolable) where T : IPoolable, new() {
        InstancePool<T>.Rest(poolable);
    }
}