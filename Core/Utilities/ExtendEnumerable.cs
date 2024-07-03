using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aequu2.Core.Utilities;

public static class ExtendEnumerable {
    public static IEnumerable<T2> SelectWhereOfType<T2>(this IEnumerable en) where T2 : class {
        foreach (object item in en) {
            if (item is T2) {
                yield return item as T2;
            }
        }
    }
    public static IEnumerable<T2> SelectWhereOfType<T, T2>(this IEnumerable<T> en) where T2 : class {
        return en.Where(v => v is T2).Select(v => v as T2);
    }

    /// <summary>Forces an Enumeration.</summary>
    /// <returns>Whether an instance of <paramref name="en2"/> is in <paramref name="en"/>.</returns>
    public static bool Match<T>(this IEnumerable<T> en, T en2) {
        return en.Any((t) => t.Equals(en2));
    }
}