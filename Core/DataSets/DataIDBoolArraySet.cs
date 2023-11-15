using ReLogic.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Terraria;

namespace Aequus.Core.DataSets;

public sealed class DataIDBoolArraySet : ICollection<string> {
    private bool[] _values = Array.Empty<bool>();
    public bool[] Values { get => _values; private set => _values = value; }

    [JsonIgnore]
    public readonly HashSet<string> Data = new();

    [JsonIgnore]
    public readonly IdDictionary idDictionary;

    public int Count => Data.Count;

    public bool IsReadOnly => false;

    public DataIDBoolArraySet(IdDictionary idDictionary) {
        this.idDictionary = idDictionary;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(string name) {
        return Data.Contains(name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int id) {
        return _values[id];
    }

    public bool this[int id] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _values[id];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _values[id] = value;
    }

    public void CheckCapacity(int id) {
        if (_values.Length <= id) {
            Array.Resize(ref _values, id + 1);
        }
    }

    public void Add(string name) {
        Data.Add(name);
        if (idDictionary.TryGetId(name, out int id)) {
            CheckCapacity(id);
            _values[id] = true;
        }
#if DEBUG
        else {
            Aequus.Instance.Logger.Error($"Failed adding {idDictionary.GetType().Name} '{name}'. Does not have an existing game ID.");
        }
#endif
    }

    public void Add(int id) {
        CheckCapacity(id);
        _values[id] = true;
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
        for (int i = 0; i < _values.Length; i++) {
            _values[i] = false;
        }
    }

    public void CopyTo(string[] array, int arrayIndex) {
        Data.CopyTo(array, arrayIndex);
    }

    public void CopyTo(bool[] array, int arrayIndex) {
        _values.CopyTo(array, arrayIndex);
    }

    public bool Remove(string name) {
        if (idDictionary.TryGetId(name, out int id)) {
            if (_values.IndexInRange(id)) {
                _values[id] = false;
            }
        }
        return Data.Remove(name);
    }

    public bool Remove(int id) {
        bool returnValue = false;
        if (_values.IndexInRange(id)) {
            returnValue = _values[id];
            _values[id] = false;
        }
        if (idDictionary.TryGetName(id, out var name)) {
            Data.Remove(name);
        }
        else {
            //throw new Exception($"Name is not expressed in the ID set.");
        }
        return returnValue;
    }

    public IEnumerator<string> GetEnumerator() {
        return (Data as IEnumerable<string>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return (Data as IEnumerable).GetEnumerator();
    }

    public static implicit operator bool[](DataIDBoolArraySet valueSet) {
        return valueSet._values;
    }

    public static implicit operator HashSet<string>(DataIDBoolArraySet valueSet) {
        return valueSet.Data;
    }
}