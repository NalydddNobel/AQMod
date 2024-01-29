using System;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Core.Utilities;

public static class ExtendEnumerable {
    public static bool Match<T>(this IEnumerable<T> en, T en2) {
        return en.Any((t) => t.Equals(en2));
    }
}