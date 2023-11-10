using Newtonsoft.Json;
using ReLogic.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

public sealed class DataIDKeyDictionary<T> : IDictionary<string, T> {
    [JsonIgnore]
    public readonly Dictionary<int, T> DataById = new();
    [JsonProperty]
    public readonly Dictionary<string, T> Data = new();

    [JsonIgnore]
    public readonly IdDictionary idDictionary;

    public int Count => Data.Count;

    public bool IsReadOnly => false;

    public ICollection<string> Keys => Data.Keys;

    public ICollection<T> Values => Data.Values;

    public T this[int id] {
        get => DataById[id];
        set {
            if (idDictionary.TryGetName(id, out string name)) {
                Data[name] = value;
            }
            DataById[id] = value;
        }
    }
    public T this[string key] {
        get => Data[key];
        set {
            if (idDictionary.TryGetId(key, out int id)) {
                DataById[id] = value;
            }
            Data[key] = value;
        }
    }

    public DataIDKeyDictionary(IdDictionary idDictionary) {
        this.idDictionary = idDictionary;
    }

    public void Add(int id, T value) {
        if (idDictionary.TryGetName(id, out string name)) {
            Data.Add(name, value);
        }
        DataById.Add(id, value);
    }

    public void Add(string key, T value) {
        if (idDictionary.TryGetId(key, out int id)) {
            DataById.Add(id, value);
        }
        Data.Add(key, value);
    }

    public bool ContainsId(int id) {
        return DataById.ContainsKey(id);
    }

    public bool ContainsKey(string key) {
        return Data.ContainsKey(key);
    }

    public bool Remove(string key) {
        if (idDictionary.TryGetId(key, out int id)) {
            DataById.Remove(id);
        }
        return Data.Remove(key);
    }

    public bool TryGetValue(int key, [MaybeNullWhen(false)] out T value) {
        return DataById.TryGetValue(key, out value);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value) {
        return Data.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<string, T> item) {
        Add(item.Key, item.Value);
    }

    public void Clear() {
        Data.Clear();
        DataById.Clear();
    }

    public bool Contains(KeyValuePair<string, T> item) {
        return (Data as IDictionary<string, T>).Contains(item);
    }

    public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex) {
        (Data as IDictionary<string, T>).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, T> item) {
        if (idDictionary.TryGetId(item.Key, out int id)) {
            (DataById as IDictionary<int, T>).Remove(new KeyValuePair<int, T>(id, item.Value));
        }
        return (Data as IDictionary<string, T>).Remove(item);
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator() {
        return (Data as IEnumerable<KeyValuePair<string, T>>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return (Data as IEnumerable).GetEnumerator();
    }
}