using Aequus.Common.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Common.DataSets;

/// <summary>
/// Represents an entry for a piece of content which uses an Id system.
/// <para>
/// <typeparamref name="T"/> should be one of the "X-ID" classes, like <see cref="ItemID"/>, <see cref="BuffID"/>, ect.
/// The ID class must also have an <see cref="ReLogic.Reflection.IdDictionary"/> named "Search", and a value for the total amount of vanilla entries labled "Count" (which can also be converted into an <see cref="int"/>)
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
[JsonConverter(typeof(EntryConverter))]
public struct IDEntry<T> : IIDEntry where T : class {
    public const int InvalidEntryOffset = -1000;

    /// <summary><inheritdoc cref="IIDEntry.Name"/> Automatically grabbed from <typeparamref name="T"/>.Search.GetName(<see cref="Id"/>). Returns "Unknown" or a name of an invalid entry if no name exists.</summary>
    [JsonProperty]
    public readonly string Name => IDCommons<T>.Search.TryGetName(Id, out string name) ? name :
        _unloadedNames.TryGetValue(Id, out string? unloadedName) ? unloadedName : "Unknown";

    public readonly bool ValidEntry => Id > IDCommons<T>.StartCount;

    /// <summary><inheritdoc cref="IIDEntry.VanillaEntry"/> Automatically determined by checking if <see cref="Id"/> is less than <typeparamref name="T"/>.Count.</summary>
    public readonly bool VanillaEntry => Id < IDCommons<T>.Count;

    public int Id { get; private set; }

    private static Dictionary<int, string> _unloadedNames = [];

    public void SetId(int id) {
        Id = id;
    }

    public void SetName(string name) {
        if (IDCommons<T>.Search.TryGetId(name, out int id)) {
            Id = id;
            return;
        }

        // If no entry exists in the IdDictionary, assign it to an unloaded name.
        // The unloadedNames array pools all of the names so there doesn't need to be duplicate strings.

        // Set the Id.
        Id = InvalidEntryOffset - id - _unloadedNames.Count;

        _unloadedNames.Add(Id, name);
    }

    #region System Methods
    public override readonly bool Equals([NotNullWhen(true)] object? obj) {
        return obj is not IDEntry<T> otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override readonly int GetHashCode() {
        return Id;
    }

    public override readonly string ToString() {
        return Name;
    }

    public static implicit operator string(IDEntry<T> entry) {
        return entry.Name;
    }

    public static implicit operator int(IDEntry<T> entry) {
        return entry.Id;
    }

    public static implicit operator IDEntry<T>(int id) {
        IDEntry<T> entry = new IDEntry<T>();
        entry.SetId(id);
        return entry;
    }

    public static explicit operator IDEntry<T>(string name) {
        IDEntry<T> entry = new IDEntry<T>();
        entry.SetName(name);
        return entry;
    }

    public static bool operator ==(IDEntry<T> left, IDEntry<T> right) {
        return left.Equals(right);
    }

    public static bool operator !=(IDEntry<T> left, IDEntry<T> right) {
        return !(left == right);
    }
    #endregion
}
