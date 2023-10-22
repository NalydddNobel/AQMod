using ReLogic.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Aequus.Core.DataSets;

public class DataIDValueSet : ICollection<string> {
    [JsonIgnore]
    public readonly List<int> ValueList = new();
    [JsonIgnore]
    public readonly HashSet<int> ValueLookups = new();
    [JsonIgnore]
    public readonly HashSet<string> Data = new();

    [JsonIgnore]
    public readonly IdDictionary idDictionary;

    public int Count => ValueList.Count;

    public bool IsReadOnly => false;

    public DataIDValueSet(IdDictionary idDictionary) {
        this.idDictionary = idDictionary;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(string name) {
        return Data.Contains(name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int id) {
        return ValueLookups.Contains(id);
    }

    public void Add(string name) {
        Data.Add(name);
        if (idDictionary.TryGetId(name, out int id)) {
            ValueList.Add(id);
            ValueLookups.Add(id);
        }
#if DEBUG
        else {
            Aequus.Instance.Logger.Error($"Failed adding {idDictionary.GetType().Name} '{name}'. Does not have an existing game ID.");
        }
#endif
    }

    public void Add(int id) {
        ValueList.Add(id);
        ValueLookups.Add(id);
        if (idDictionary.TryGetName(id, out var name)) {
            Data.Add(name);
        }
#if DEBUG
        else {
            Aequus.Instance.Logger.Error($"Failed adding {idDictionary.GetType().Name} '{id}'. Does not have an existing entry name.");
            //throw new Exception($"Name is not expressed in the ID set.");
        }
#endif
    }

    public void Clear() {
        Data.Clear();
        ValueList.Clear();
        ValueLookups.Clear();
    }

    public void CopyTo(string[] array, int arrayIndex) {
        Data.CopyTo(array, arrayIndex);
    }

    public void CopyTo(int[] array, int arrayIndex) {
        ValueList.CopyTo(array, arrayIndex);
    }

    public bool Remove(string name) {
        if (idDictionary.TryGetId(name, out int id)) {
            ValueList.Remove(id);
            ValueLookups.Remove(id);
        }
        return Data.Remove(name);
    }

    public bool Remove(int id) {
        ValueLookups.Remove(id);
        if (idDictionary.TryGetName(id, out var name)) {
            Data.Remove(name);
        }
        else {
            //throw new Exception($"Name is not expressed in the ID set.");
        }
        return ValueList.Remove(id);
    }

    public IEnumerator<int> GetIntEnumerator() {
        return ValueList.GetEnumerator();
    }

    public IEnumerator<string> GetEnumerator() {
        return (Data as IEnumerable<string>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return (Data as IEnumerable).GetEnumerator();
    }

    public static implicit operator List<int>(DataIDValueSet valueSet) {
        return valueSet.ValueList;
    }

    public static implicit operator HashSet<int>(DataIDValueSet valueSet) {
        return valueSet.ValueLookups;
    }

    public static implicit operator HashSet<string>(DataIDValueSet valueSet) {
        return valueSet.Data;
    }
}