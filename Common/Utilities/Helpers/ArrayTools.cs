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

    public static void Populate<T>(T[] array, Func<int, T> Factory) {
        for (int i = 0; i < array.Length; i++) {
            array[i] = Factory(i);
        }
    }
}
