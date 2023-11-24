using Aequus.Core;
using Aequus.Core.DataSets;
using System.Collections.Generic;

namespace Aequus;

public static class DataSetExtensions {
    public static bool TryGetValue<T, TKey, TValue>(this IDictionary<T, TValue> dictionary, TKey id, out TValue value) where T : IDataEntry<TKey>, new() {
        return dictionary.TryGetValue(new() { Id = id }, out value);
    }
    public static bool ContainsKey<T, TKey, TValue>(this IDictionary<T, TValue> dictionary, TKey id) where T : IDataEntry<TKey>, new() {
        return dictionary.ContainsKey(new() { Id = id });
    }

    public static void AddEntry<T, T2>(this ICollection<T> list, T2 id) where T : IDataEntry<T2>, new() {
        ContentInitializationSteps.EnqueuePostSetupContent(() => {
            var value = new T() { Id = id };
            value.Initialize();
            list.Add(value);
        });
    }

    public static bool Contains<T, T2>(this ICollection<T> list, T2 id) where T : IDataEntry<T2>, new() {
        return list.Contains(new() { Id = id });
    }
}