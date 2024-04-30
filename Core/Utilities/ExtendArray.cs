using System;
using System.Linq;

namespace Aequus.Core.Utilities;

public static class ExtendArray {
    /// <summary>Safely adds <paramref name="other"/>'s contents into <paramref name="array"/> by resizing and inserting the elements from <paramref name="other"/> into <paramref name="array"/>.</summary>
    /// <returns>The combined array.</returns>
    public static T[] AddRangeSafe<T>(T[] array, T[] other) {
        if (other == null) {
            //throw new ArgumentNullException(nameof(other));
            return array;
        }

        if (array == null) {
            return Copy(other);
        }

        int start = array.Length;
        Array.Resize(ref array, array.Length + other.Length);

        int i = start;
        int k = 0;
        while (i < array.Length) {
            array[i] = other[k];
        }

        return array;
    }

    /// <returns>A new array with the same contents.</returns>
    public static T[] Copy<T>(this T[] array) {
        var array2 = new T[array.Length];
        array.CopyTo(array2, 0);
        return array2;
    }

    public static void ResizeAndPopulate<T>(ref T[] array, int length, Func<T> populationFactory) {
        if (array == null) {
            throw new ArgumentNullException(nameof(array));
        }
        int oldLength = array.Length;
        Array.Resize(ref array, length);
        for (int k = oldLength; k < length; k++) {
            array[k] = populationFactory();
        }
    }

    /// <returns>An array with the Length of <paramref name="length"/>, populated using <paramref name="dataFactory"/> where <see cref="int"/> is the index of the array being populated and <typeparamref name="T"/> is the value stored.</returns>
    public static T[] CreateArray<T>(Func<int, T> dataFactory, int length) {
        var arr = new T[length];
        for (int i = 0; i < length; i++) {
            arr[i] = dataFactory(i);
        }
        return arr;
    }

    /// <summary>Removes at <paramref name="index"/> and resizes <paramref name="arr"/> to fit.</summary>
    public static void RemoveAt<T>(ref T[] arr, int index) {
        var list = arr.ToList();
        list.RemoveAt(index);
        arr = list.ToArray();
    }

    /// <summary>Removes <paramref name="value"/> from <paramref name="arr"/>, and resizes <paramref name="arr"/> to fit if necessary.</summary>
    /// <returns>Whether anything was removed at all.</returns>
    public static bool Remove<T>(ref T[] arr, T value) {
        var list = arr.ToList();
        bool remove = list.Remove(value);
        arr = list.ToArray();
        return remove;
    }
}
