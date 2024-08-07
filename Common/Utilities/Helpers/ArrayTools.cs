using System;

namespace Aequus.Common.Utilities.Helpers;

public sealed class ArrayTools {
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
