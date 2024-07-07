namespace AequusRemake.Core.Util.Extensions;

public static class ArrayPoolExtensions {
    /// <summary>Clears <paramref name="rent"/> and returns it to <paramref name="pool"/>.</summary>
    public static void ClearReturn<T>(this FNAUtils.ArrayPool<T> pool, T[] rent) {
        for (int i = 0; i < rent.Length; i++) {
            rent[i] = default(T);
        }
        pool.Return(rent);
    }
}
