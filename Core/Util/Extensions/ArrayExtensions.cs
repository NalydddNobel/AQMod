using System;
using System.Linq;

namespace AequusRemake.Core.Util.Extensions;

public static class ArrayExtensions {
    /// <returns>A new array with the same contents.</returns>
    public static T[] NewClone<T>(this T[] array) {
        var array2 = new T[array.Length];
        array.CopyTo(array2, 0);
        return array2;
    }

    public static void ResizeAndPopulate<T>(ref T[] array, int length, Func<T> populationFactory) {
        ArgumentNullException.ThrowIfNull(array, nameof(array));

        int oldLength = array.Length;
        Array.Resize(ref array, length);

        for (int k = oldLength; k < length; k++) {
            array[k] = populationFactory();
        }
    }

    /// <summary>Removes at <paramref name="index"/> and resizes <paramref name="arr"/> to fit.</summary>
    public static void RemoveAt<T>(ref T[] arr, int index) {
        var list = arr.ToList();
        list.RemoveAt(index);
        arr = [.. list];
    }

    /// <summary>Removes <paramref name="value"/> from <paramref name="arr"/>, and resizes <paramref name="arr"/> to fit if necessary.</summary>
    /// <returns>Whether anything was removed at all.</returns>
    public static bool Remove<T>(ref T[] arr, T value) {
        var list = arr.ToList();
        bool remove = list.Remove(value);
        arr = [.. list];
        return remove;
    }
}
