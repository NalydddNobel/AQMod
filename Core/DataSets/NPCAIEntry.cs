using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<NPCAIEntry, System.Int32>))]
public struct NPCAIEntry : IDataEntry<System.Int32> {
    [JsonProperty]
    public System.String Name { get; set; }

    [JsonIgnore]
    public System.Int32 Id { get; set; }

    [JsonIgnore]
    public System.Boolean ValidEntry => Id >= 0;

    [JsonIgnore]
    public System.Boolean VanillaEntry => Id >= 0;

    private static System.Int32 _uniqueIds = -1;

    public NPCAIEntry(System.String name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public NPCAIEntry(System.Int32 id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override System.Boolean Equals([NotNullWhen(true)] System.Object obj) {
        return obj is not NPCAIEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override System.Int32 GetHashCode() {
        return Id;
    }

    public override System.String ToString() {
        return Name;
    }

    public void Initialize() {
        if (!System.String.IsNullOrEmpty(Name)) {
            Id = NPCAIStyleID.Search.TryGetId(Name, out System.Int32 id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = NPCAIStyleID.Search.TryGetName(Id, out System.String name) ? name : "Unknown";
        }
    }

    public static implicit operator System.String(NPCAIEntry entry) {
        return entry.Name;
    }

    public static implicit operator System.Int32(NPCAIEntry entry) {
        return entry.Id;
    }

    public static explicit operator NPCAIEntry(System.Int32 id) {
        return new(id);
    }

    public static explicit operator NPCAIEntry(System.String name) {
        return new(name);
    }

    public static System.Boolean operator ==(NPCAIEntry left, NPCAIEntry right) {
        return left.Equals(right);
    }

    public static System.Boolean operator !=(NPCAIEntry left, NPCAIEntry right) {
        return !(left == right);
    }
}