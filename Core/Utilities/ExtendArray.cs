using System;
using System.Linq;

namespace Aequus.Core.Utilities;

public static class ExtendArray {
    /// <returns>A new array with the same contents.</returns>
    public static T[] Copy<T>(this T[] array) {
        var array2 = new T[array.Length];
        array.CopyTo(array2, 0);
        return array2;
    }

    /// <returns>An array with the Length of <paramref name="length"/>, populated using <paramref name="dataFactory"/> (<see cref="Func{Int32, T}"/>) where <see cref="Int32"/> is the index of the array being populated and <typeparamref name="T"/> is the value stored.</returns>
    public static T[] CreateArray<T>(Func<Int32, T> dataFactory, Int32 length) {
        var arr = new T[length];
        for (Int32 i = 0; i < length; i++) {
            arr[i] = dataFactory(i);
        }
        return arr;
    }

    /// <summary>Removes at <paramref name="index"/> and resizes <paramref name="arr"/> to fit.</summary>
    public static void RemoveAt<T>(ref T[] arr, Int32 index) {
        var list = arr.ToList();
        list.RemoveAt(index);
        arr = list.ToArray();
    }

    /// <summary>Removes <paramref name="value"/> from <paramref name="arr"/>, and resizes <paramref name="arr"/> to fit if necessary.</summary>
    /// <returns>Whether anything was removed at all.</returns>
    public static Boolean Remove<T>(ref T[] arr, T value) {
        var list = arr.ToList();
        Boolean remove = list.Remove(value);
        arr = list.ToArray();
        return remove;
    }
}
