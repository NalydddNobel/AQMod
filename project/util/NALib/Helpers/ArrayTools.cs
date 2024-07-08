using NALib.Extensions;
using System;

namespace NALib.Helpers;

public sealed class ArrayTools {
    /// <summary>Resizes <paramref name="array"/> and adds <paramref name="value"/> to the end of the array.</summary>
    public static void AllocAdd<T>(ref T[]? array, T value) {
        ArgumentNullException.ThrowIfNull(array, nameof(array));

        Array.Resize(ref array, array.Length + 1);

        array[^1] = value;
    }

    /// <summary>Safely adds <paramref name="other"/>'s contents into <paramref name="array"/> by resizing and inserting the elements from <paramref name="other"/> into <paramref name="array"/>.</summary>
    /// <returns>The combined array.</returns>
    public static T[]? AllocAddRange<T>(T[]? array, T[]? other) {
        if (other == null) {
            return array;
        }

        if (array == null) {
            return other.NewClone();
        }

        int start = array.Length;
        Array.Resize(ref array, array.Length + other.Length);

        int i = start;
        int k = 0;
        while (i < array.Length) {
            array[i] = other[k];
            i++;
            k++;
        }

        return array;
    }

    /// <returns>An array with the Length of <paramref name="length"/>, populated using <paramref name="dataFactory"/> where <see cref="int"/> is the index of the array being populated and <typeparamref name="T"/> is the value stored.</returns>
    public static T[]? PopulateNewArray<T>(Func<int, T> dataFactory, int length) {
        ArgumentNullException.ThrowIfNull(dataFactory, nameof(dataFactory));

        var arr = new T[length];
        for (int i = 0; i < length; i++) {
            arr[i] = dataFactory(i);
        }
        return arr;
    }
}
