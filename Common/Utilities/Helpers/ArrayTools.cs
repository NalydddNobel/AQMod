using System;

namespace Aequus.Common.Utilities.Helpers;

public sealed class ArrayTools {
    /// <exception cref="ArgumentNullException"></exception>
    public static void ResizeAndPopulate<T>(ref T[] array, int length, Func<T> populationFactory) {
        ArgumentNullException.ThrowIfNull(array, nameof(array));
        ArgumentNullException.ThrowIfNull(populationFactory, nameof(populationFactory));

        int oldLength = array.Length;
        Array.Resize(ref array, length);

        for (int k = oldLength; k < length; k++) {
            array[k] = populationFactory();
        }
    }

    public static T[] New<T>(int length, Func<int, T> Factory) {
        T[] newArray = new T[length];
        Populate(newArray, Factory);
        return newArray;
    }

    public static T[,] New2D<T>(int length1D, int length2D, Func<int, int, T> Factory) {
        T[,] newArray = new T[length1D, length2D];

        for (int x = 0; x < length1D; x++) {
            for (int y = 0; y < length2D; y++) {
                newArray[x, y] = Factory(x, y);
            }
        }

        return newArray;
    }

    public static void Populate<T>(T[] array, Func<int, T> Factory) {
        for (int i = 0; i < array.Length; i++) {
            array[i] = Factory(i);
        }
    }
}
