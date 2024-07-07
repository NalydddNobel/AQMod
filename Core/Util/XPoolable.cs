using AequusRemake.Core.Structures.Pooling;

namespace AequusRemake.Core.Util;

public static class XPoolable {
    public static void Rest<T>(this T poolable) where T : IPoolable, new() {
        InstancePool<T>.Rest(poolable);
    }
}