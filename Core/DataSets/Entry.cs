using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

public interface IEntry {
    /// <summary>Name for the content entry.</summary>
    public string Name { get; }
    /// <summary>ID for the content.</summary>
    public int Id { get; }
    /// <summary>Whether this is a valid entry. Entries for content from other mods will be marked as Invalid if the mod is not enabled.</summary>
    bool ValidEntry { get; }
    /// <summary>Whether this is a vanilla entry.</summary>
    bool VanillaEntry { get; }

    /// <summary>Set <see cref="Id"/> using <paramref name="name"/>. Called when loading the .json metadata files.</summary>
    /// <param name="name">Name of the content.</param>
    void SetName(string name);

    /// <summary>Set <see cref="Id"/> directly. Use this to update the name or other values if needed.</summary>
    /// <param name="id">The Content's Id.</param>
    void SetId(int id);
}

/// <summary>
/// Represents an entry for a piece of content which uses an Id system.
/// <para>
/// <typeparamref name="T"/> should be one of the "X-ID" classes, like <see cref="ItemID"/>, <see cref="BuffID"/>, ect.
/// The ID class must also have an <see cref="ReLogic.Reflection.IdDictionary"/> named "Search", and a value for the total amount of vanilla entries labled "Count" (which can also be converted into an <see cref="int"/>)
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
[JsonConverter(typeof(EntryConverter))]
public struct Entry<T> : IEntry where T : class {
    public const int InvalidEntryOffset = -1000;

    /// <summary><inheritdoc cref="IEntry.Name"/> Automatically grabbed from <typeparamref name="T"/>.Search.GetName(<see cref="Id"/>). Returns "Unknown" or a name of an invalid entry if no name exists.</summary>
    [JsonProperty]
    public string Name => IDCommons<T>.Search.TryGetName(Id, out string name) ? name : 
        _unloadedNames.IndexInRange(-Id + InvalidEntryOffset) ? _unloadedNames[-Id + InvalidEntryOffset] : "Unknown";

    public bool ValidEntry => Id > 0 && Id < IDCommons<T>.Count;

    /// <summary><inheritdoc cref="IEntry.VanillaEntry"/> Automatically determined by checking if <see cref="Id"/> is less than <typeparamref name="T"/>.Count.</summary>
    public bool VanillaEntry => Id < IDCommons<T>.Count;

    public int Id { get; private set; }

    private static string[] _unloadedNames = Array.Empty<string>();

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

        // Check for an existing match first.
        int match = Array.IndexOf(_unloadedNames, name);

        if (match != -1) {
            // If there is a match, assign the id to the name's array index.
            id = match;
        }
        else {
            // Otherwise create a new entry for this invalid name.
            id = _unloadedNames.Length;
            Array.Resize(ref _unloadedNames, _unloadedNames.Length + 1);
            _unloadedNames[^1] = name;
        }

        // Set the Id.
        Id = InvalidEntryOffset - id;
    }

    #region System Methods
    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not Entry<T> otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public static implicit operator string(Entry<T> entry) {
        return entry.Name;
    }

    public static implicit operator int(Entry<T> entry) {
        return entry.Id;
    }

    public static implicit operator Entry<T>(int id) {
        Entry<T> entry = new Entry<T>();
        entry.SetId(id);
        return entry;
    }

    public static explicit operator Entry<T>(string name) {
        Entry<T> entry = new Entry<T>();
        entry.SetName(name);
        return entry;
    }

    public static bool operator ==(Entry<T> left, Entry<T> right) {
        return left.Equals(right);
    }

    public static bool operator !=(Entry<T> left, Entry<T> right) {
        return !(left == right);
    }
    #endregion
}

internal class EntryConverter : JsonConverter<IEntry> {
    public override void WriteJson(JsonWriter writer, [AllowNull] IEntry value, JsonSerializer serializer) {
        writer.WriteValue(value.Name);
        //if (value.ValidEntry && value.VanillaEntry) {
        //    writer.WriteComment(value.Id.ToString());
        //}
    }

    public override IEntry ReadJson(JsonReader reader, Type objectType, [AllowNull] IEntry existingValue, bool hasExistingValue, JsonSerializer serializer) {
        string name = (string)reader.Value;
        IEntry entry = (IEntry)Activator.CreateInstance(objectType);

        entry.SetName(name);

        return entry;
    }
}