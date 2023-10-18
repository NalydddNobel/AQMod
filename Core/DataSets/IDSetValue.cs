using ReLogic.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Aequus.Core.DataSets;

public class IDSetValue : ICollection<string> {
    [JsonIgnore]
    private readonly List<int> ValueList = new();
    [JsonIgnore]
    private readonly HashSet<int> ValueLookups = new();
    [JsonIgnore]
    public static readonly HashSet<string> Data = new();

    [JsonIgnore]
    public readonly IdDictionary idDictionary;

    public int Count => ValueList.Count;

    public bool IsReadOnly => false;

    internal IDSetValue(IdDictionary idDictionary) {
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
    }

    public void Add(int id) {
        if (!idDictionary.TryGetName(id, out var name)) {
            throw new Exception($"Name is not expressed in the ID set.");
        }
        Data.Add(name);
        ValueList.Add(id);
        ValueLookups.Add(id);
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
        if (!idDictionary.TryGetName(id, out var name)) {
            throw new Exception($"Name is not expressed in the ID set.");
        }
        ValueList.Remove(id);
        ValueLookups.Remove(id);
        return Data.Remove(name);
    }

    public IEnumerator<int> GetIntEnumerator() {
        return ValueList.GetEnumerator();
    }

    public IEnumerator<string> GetEnumerator() {
        return Data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return Data.GetEnumerator();
    }
}