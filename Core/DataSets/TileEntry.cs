using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<TileEntry, int>))]
public struct TileEntry : IDataEntry<int> {
    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => Id >= 0;

    [JsonIgnore]
    public bool VanillaEntry => Id < TileID.Count;

    private static int _uniqueIds = -1;

    public TileEntry(string name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public TileEntry(int id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not TileEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (!string.IsNullOrEmpty(Name)) {
            Id = TileID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = TileID.Search.TryGetName(Id, out string name) ? name : "Unknown";
        }
    }

    public static implicit operator string(TileEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(TileEntry entry) {
        return entry.Id;
    }

    public static explicit operator TileEntry(int id) {
        return new(id);
    }

    public static explicit operator TileEntry(string name) {
        return new(name);
    }

    public static bool operator ==(TileEntry left, TileEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(TileEntry left, TileEntry right) {
        return !(left == right);
    }
}