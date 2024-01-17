using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<ItemEntry, int>))]
public struct ItemEntry : IDataEntry<int> {
    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => Id > ItemID.None;

    [JsonIgnore]
    public bool VanillaEntry => Id < ItemID.Count;

    private static int _uniqueIds;

    public ItemEntry(string name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public ItemEntry(int id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not ItemEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (!string.IsNullOrEmpty(Name)) {
            Id = ItemID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = ItemID.Search.TryGetName(Id, out string name) ? name : "Unknown";
        }
    }

    public static implicit operator string(ItemEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(ItemEntry entry) {
        return entry.Id;
    }

    public static explicit operator ItemEntry(int id) {
        return new(id);
    }

    public static explicit operator ItemEntry(string name) {
        return new(name);
    }

    public static bool operator ==(ItemEntry left, ItemEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(ItemEntry left, ItemEntry right) {
        return !(left == right);
    }
}