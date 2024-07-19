using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.Utilities.Extensions;

public static class EnumerableExtensions {
    /// <summary>Allocates a lookup table for values->index for the <paramref name="array"/>. This is useful for creating reverse lookups for datasets.</summary>
    /// <param name="array">The Array.</param>
    /// <param name="throwOnDuplicates">Whether or not to throw an error if any duplicate values are found. If this is false, the lookup will point to the last duplicate value found in the array.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public static Dictionary<T, int> AllocLookup<T>(this T[] array, bool throwOnDuplicates = false) {
        Dictionary<T, int> lookup = new(array.Length);

        for (int i = 0; i < array.Length; i++) {
            T value = array[i];

            if (throwOnDuplicates && lookup.ContainsKey(value)) {
                throw new System.Exception($"Duplicate value found when generating array index lookup. ({value})");
            }

            lookup[array[i]] = i;
        }

        return lookup;
    }

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