using System;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Core.Utilities;

public static class ExtendEnumerable {
    /// <summary>Forces an Enumeration.</summary>
    /// <returns>Whether an instance of <paramref name="en2"/> is in <paramref name="en"/>.</returns>
    public static Boolean Match<T>(this IEnumerable<T> en, T en2) {
        return en.Any((t) => t.Equals(en2));
    }
}