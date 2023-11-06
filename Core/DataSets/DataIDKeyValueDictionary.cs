using Newtonsoft.Json;
using ReLogic.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Aequus.Core.DataSets;

public class DataIDKeyValueDictionary : IDictionary<string, string> {
    [JsonIgnore]
    public readonly Dictionary<int, int> IdDataById = new();
    [JsonIgnore]
    public readonly Dictionary<string, int> IdDataByName = new();
    [JsonIgnore]
    public readonly Dictionary<int, string> DataById = new();
    [JsonProperty]
    public readonly Dictionary<string, string> Data = new();

    [JsonIgnore]
    public readonly IdDictionary keyIdDictionary;
    [JsonIgnore]
    public readonly IdDictionary valueIdDictionary;

    public int Count => Data.Count;

    public bool IsReadOnly => false;

    public ICollection<string> Keys => Data.Keys;

    public ICollection<string> Values => Data.Values;

    public int this[int id] {
        get => IdDataById[id];
        set {
            keyIdDictionary.TryGetName(id, out string keyName);
            valueIdDictionary.TryGetName(value, out string valueName);
            if (keyName != default) {
                IdDataByName[keyName] = value;
            }
            if (valueName != default) {
                DataById[id] = valueName;
            }
            if (keyName != default && valueName != default) {
                Data[keyName] = valueName;
            }
            IdDataById[id] = value;
        }
    }
    public string this[string key] {
        get => Data[key];
        set {
            keyIdDictionary.TryGetId(key, out int keyId);
            valueIdDictionary.TryGetId(value, out int valueId);
            if (keyId != default) {
                DataById[keyId] = value;
            }
            if (valueId != default) {
                IdDataByName[key] = valueId;
            }
            if (keyId != default && valueId != default) {
                IdDataById[keyId] = valueId;
            }
            Data[key] = value;
        }
    }

    public DataIDKeyValueDictionary(IdDictionary keyIdDictionary, IdDictionary valueIdDictionary) {
        this.keyIdDictionary = keyIdDictionary;
        this.valueIdDictionary = valueIdDictionary;
    }

    public void Add(int id, int value) {
        keyIdDictionary.TryGetName(id, out string keyName);
        valueIdDictionary.TryGetName(value, out string valueName);
        if (keyName != default) {
            IdDataByName.Add(keyName, value);
        }
        if (valueName != default) {
            DataById.Add(id, valueName);
        }
        if (keyName != default && valueName != default) {
            Data.Add(keyName, valueName);
        }
        IdDataById.Add(id, value);
    }

    public void Add(string key, string value) {
        keyIdDictionary.TryGetId(key, out int keyId);
        valueIdDictionary.TryGetId(value, out int valueId);
        if (keyId != default) {
            DataById.Add(keyId, value);
        }
        if (valueId != default) {
            IdDataByName.Add(key, valueId);
        }
        if (keyId != default && valueId != default) {
            IdDataById.Add(keyId, valueId);
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
        if (keyIdDictionary.TryGetId(key, out int keyId)) {
            DataById.Remove(keyId);
            IdDataById.Remove(keyId);
        }
        IdDataByName.Remove(key);
        return Data.Remove(key);
    }

    public bool TryGetIdValue(int key, [MaybeNullWhen(false)] out int value) {
        return IdDataById.TryGetValue(key, out value);
    }

    public bool TryGetIdValue(string key, [MaybeNullWhen(false)] out int value) {
        return IdDataByName.TryGetValue(key, out value);
    }

    public bool TryGetValue(int key, [MaybeNullWhen(false)] out string value) {
        return DataById.TryGetValue(key, out value);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) {
        return Data.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<string, string> item) {
        Add(item.Key, item.Value);
    }

    public void Clear() {
        Data.Clear();
        DataById.Clear();
    }

    public bool Contains(KeyValuePair<string, string> item) {
        return (Data as IDictionary<string, string>).Contains(item);
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
        (Data as IDictionary<string, string>).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, string> item) {
        if (keyIdDictionary.TryGetId(item.Key, out int id)) {
            (DataById as IDictionary<int, string>).Remove(new KeyValuePair<int, string>(id, item.Value));
        }
        return (Data as IDictionary<string, string>).Remove(item);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
        return (Data as IEnumerable<KeyValuePair<string, string>>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return (Data as IEnumerable).GetEnumerator();
    }
}

public class DataIDKeyValueAttribute : Attribute {
    private readonly Type keyIdSet;
    private readonly Type valueIdSet;

    public DataIDKeyValueAttribute(Type keyIdSet, Type valueIdSet) {
        this.keyIdSet = keyIdSet;
        this.valueIdSet = valueIdSet;
    }

    public IdDictionary GetKeyIdDictionary() {
        return (IdDictionary)keyIdSet.GetField("Search", BindingFlags.Public | BindingFlags.Static).GetValue(null);
    }
    public IdDictionary GetValueIdDictionary() {
        return (IdDictionary)valueIdSet.GetField("Search", BindingFlags.Public | BindingFlags.Static).GetValue(null);
    }
}