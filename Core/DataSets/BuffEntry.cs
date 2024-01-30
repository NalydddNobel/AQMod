using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<BuffEntry, System.Int32>))]
public struct BuffEntry : IDataEntry<System.Int32> {
    [JsonProperty]
    public System.String Name { get; set; }

    [JsonIgnore]
    public System.Int32 Id { get; set; }

    [JsonIgnore]
    public System.Boolean ValidEntry => true;

    [JsonIgnore]
    public System.Boolean VanillaEntry => Id < BuffID.Count;

    private static System.Int32 _uniqueIds = System.Int32.MaxValue;

    public BuffEntry(System.String name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public BuffEntry(System.Int32 id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override System.Boolean Equals([NotNullWhen(true)] System.Object obj) {
        return obj is not BuffEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override System.Int32 GetHashCode() {
        return Id;
    }

    public override System.String ToString() {
        return Name;
    }

    public void Initialize() {
        if (!System.String.IsNullOrEmpty(Name)) {
            Id = BuffID.Search.TryGetId(Name, out System.Int32 id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = BuffID.Search.TryGetName(Id, out System.String name) ? name : "Unknown";
        }
    }

    public static implicit operator System.String(BuffEntry entry) {
        return entry.Name;
    }

    public static implicit operator System.Int32(BuffEntry entry) {
        return entry.Id;
    }

    public static explicit operator BuffEntry(System.Int32 id) {
        return new(id);
    }

    public static explicit operator BuffEntry(System.String name) {
        return new(name);
    }

    public static System.Boolean operator ==(BuffEntry left, BuffEntry right) {
        return left.Equals(right);
    }

    public static System.Boolean operator !=(BuffEntry left, BuffEntry right) {
        return !(left == right);
    }
}