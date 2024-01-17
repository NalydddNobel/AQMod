using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<BuffEntry, int>))]
public struct BuffEntry : IDataEntry<int> {
    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => true;

    [JsonIgnore]
    public bool VanillaEntry => Id < BuffID.Count;

    private static int _uniqueIds = int.MaxValue;

    public BuffEntry(string name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public BuffEntry(int id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not BuffEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (!string.IsNullOrEmpty(Name)) {
            Id = BuffID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = BuffID.Search.TryGetName(Id, out string name) ? name : "Unknown";
        }
    }

    public static implicit operator string(BuffEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(BuffEntry entry) {
        return entry.Id;
    }

    public static explicit operator BuffEntry(int id) {
        return new(id);
    }

    public static explicit operator BuffEntry(string name) {
        return new(name);
    }

    public static bool operator ==(BuffEntry left, BuffEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(BuffEntry left, BuffEntry right) {
        return !(left == right);
    }
}