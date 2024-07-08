using System;
using System.Linq;

namespace NALib.Extensions;

public static class ArrayExtensions {
    /// <returns>A new array with the same contents.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T[] NewClone<T>(this T[]? array) {
        ArgumentNullException.ThrowIfNull(array, nameof(array));

        var array2 = new T[array.Length];
        array.CopyTo(array2, 0);
        return array2;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public static void ResizeAndPopulate<T>(ref T[]? array, int length, Func<T> populationFactory) {
        ArgumentNullException.ThrowIfNull(array, nameof(array));
        ArgumentNullException.ThrowIfNull(populationFactory, nameof(populationFactory));

        int oldLength = array.Length;
        Array.Resize(ref array, length);

        for (int k = oldLength; k < length; k++) {
            array[k] = populationFactory();
        }
    }

    /// <summary>Removes at <paramref name="index"/> and resizes <paramref name="array"/> to fit.</summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RemoveAt<T>(ref T[]? array, int index) {
        ArgumentNullException.ThrowIfNull(array, nameof(array));

        var list = array.ToList();
        list.RemoveAt(index);
        array = [.. list];
    }

    /// <summary>Removes <paramref name="value"/> from <paramref name="array"/>, and resizes <paramref name="array"/> to fit if necessary.</summary>
    /// <returns>Whether anything was removed at all.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool Remove<T>(ref T[]? array, T value) {
        ArgumentNullException.ThrowIfNull(array, nameof(array));

        var list = array.ToList();
        bool remove = list.Remove(value);
        array = [.. list];
        return remove;
    }
}
