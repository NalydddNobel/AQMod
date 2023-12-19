using System;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Core.Utilities;

public static class EnumerableHelper {
    public static T[] CloneArray<T>(this T[] array) {
        var array2 = new T[array.Length];
        array.CopyTo(array2, 0);
        return array2;
    }

    public static T[] CreateArray<T>(Func<int, T> dataFactory, int length) {
        var arr = new T[length];
        for (int i = 0; i < length; i++) {
            arr[i] = dataFactory(i);
        }
        return arr;
    }

    public static T[] CreateArray<T>(T data, int length) {
        var arr = new T[length];
        for (int i = 0; i < length; i++) {
            arr[i] = data;
        }
        return arr;
    }

    public static bool Remove<T>(ref T[] arr, T value) {
        var list = arr.ToList();
        bool remove = list.Remove(value);
        arr = list.ToArray();
        return remove;
    }

    public static bool ContainsType<T, T2>(this IEnumerable<T> en, IEnumerable<T2> types) {
        return en.Any(v => types.Any(v2 => v.GetType() == v2.GetType()));
    }
    public static bool ContainsType<T>(this IEnumerable<T> en, Type type) {
        return en.Any(v => v.GetType() == type);
    }
    public static bool ContainsType<T, T2>(this IEnumerable<T> en) where T2 : T {
        return en.ContainsType(typeof(T2));
    }

    public static bool Any<T>(this IEnumerable<T> en, Predicate<T> predicate) {
        foreach (var t in en) {
            if (predicate(t)) {
                return true;
            }
        }
        return false;
    }
    public static bool Any<T>(this IEnumerable<T> en, T en2) {
        return en.Any((t) => t.Equals(en2));
    }
    public static bool Any<T>(this IEnumerable<T> en, params T[] en2) {
        return en.Any((t) => {
            foreach (var t2 in en2) {
                if (t.Equals(t2)) {
                    return true;
                }
            }
            return false;
        });
    }
    public static bool Any<T>(this IEnumerable<T> en, IEnumerable<T> en2) {
        return en.Any((t) => {
            foreach (var t2 in en2) {
                if (t.Equals(t2)) {
                    return true;
                }
            }
            return false;
        });
    }

    public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> enumerable) {
        foreach (var v in enumerable) {
            list.Add(v);
        }
    }
}