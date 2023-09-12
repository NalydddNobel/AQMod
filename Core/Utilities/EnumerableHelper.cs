using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aequus.Core.Utilities;

public static class EnumerableHelper {
    public static bool ContainsType<T, T2>(this IEnumerable<T> en, IEnumerable<T2> types) {
        return ContainsAny(en, v => types.ContainsAny(v2 => v.GetType() == v2.GetType()));
    }
    public static bool ContainsType<T>(this IEnumerable<T> en, Type type) {
        return ContainsAny(en, v => v.GetType() == type);
    }
    public static bool ContainsType<T, T2>(this IEnumerable<T> en) where T2 : T {
        return ContainsType(en, typeof(T2));
    }
    public static bool ContainsAny<T>(this IEnumerable<T> en, Predicate<T> predicate) {
        foreach (var t in en) {
            if (predicate(t)) {
                return true;
            }
        }
        return false;
    }
    public static bool ContainsAny<T>(this IEnumerable<T> en, T en2) {
        return ContainsAny(en, (t) => t.Equals(en2));
    }
    public static bool ContainsAny<T>(this IEnumerable<T> en, params T[] en2) {
        return ContainsAny(en, (t) => {
            foreach (var t2 in en2) {
                if (t.Equals(t2)) {
                    return true;
                }
            }
            return false;
        });
    }
    public static bool ContainsAny<T>(this IEnumerable<T> en, IEnumerable<T> en2) {
        return ContainsAny(en, (t) => {
            foreach (var t2 in en2) {
                if (t.Equals(t2)) {
                    return true;
                }
            }
            return false;
        });
    }
}