using System.Collections.Generic;

namespace Aequus.Common.Utilities.Extensions;

public static class DictionaryExtensions {
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) where TKey : notnull {
        if (dictionary.TryGetValue(key, out TValue? handledValue)) {
            return handledValue!;
        }

        return defaultValue;
    }
}
