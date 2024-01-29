using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Aequus.Core.Utilities;

public static class EnumerableHelper {
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

    public static T[] CreateArray<T>(Func<int, T> dataFactory, int length) {
        var arr = new T[length];
        for (int i = 0; i < length; i++) {
            arr[i] = dataFactory(i);
        }
        return arr;
    }

    public static bool Remove<T>(ref T[] arr, T value) {
        var list = arr.ToList();
        bool remove = list.Remove(value);
        arr = list.ToArray();
        return remove;
    }

    public static bool Match<T>(this IEnumerable<T> en, T en2) {
        return en.Any((t) => t.Equals(en2));
    }
}