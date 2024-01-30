using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<ItemEntry, System.Int32>))]
public struct ItemEntry : IDataEntry<System.Int32> {
    [JsonProperty]
    public System.String Name { get; set; }

    [JsonIgnore]
    public System.Int32 Id { get; set; }

    [JsonIgnore]
    public System.Boolean ValidEntry => Id > ItemID.None;

    [JsonIgnore]
    public System.Boolean VanillaEntry => Id < ItemID.Count;

    private static System.Int32 _uniqueIds;

    public ItemEntry(System.String name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public ItemEntry(System.Int32 id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override System.Boolean Equals([NotNullWhen(true)] System.Object obj) {
        return obj is not ItemEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override System.Int32 GetHashCode() {
        return Id;
    }

    public override System.String ToString() {
        return Name;
    }

    public void Initialize() {
        if (!System.String.IsNullOrEmpty(Name)) {
            Id = ItemID.Search.TryGetId(Name, out System.Int32 id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = ItemID.Search.TryGetName(Id, out System.String name) ? name : "Unknown";
        }
    }

    public static implicit operator System.String(ItemEntry entry) {
        return entry.Name;
    }

    public static implicit operator System.Int32(ItemEntry entry) {
        return entry.Id;
    }

    public static explicit operator ItemEntry(System.Int32 id) {
        return new(id);
    }

    public static explicit operator ItemEntry(System.String name) {
        return new(name);
    }

    public static System.Boolean operator ==(ItemEntry left, ItemEntry right) {
        return left.Equals(right);
    }

    public static System.Boolean operator !=(ItemEntry left, ItemEntry right) {
        return !(left == right);
    }
}