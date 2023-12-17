using System;
using System.Collections.Generic;
using System.Linq;

namespace Aequus;

public static class EnumHelper {
    public static IEnumerable<T> GetFlags<T>(this T value) where T : Enum {
        foreach (var e in Enum.GetValues(typeof(T)).Cast<T>()) {
            if (value.HasFlag(e)) {
                yield return e;
            }
        }
    }
}