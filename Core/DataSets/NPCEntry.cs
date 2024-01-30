using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<NPCEntry, System.Int32>))]
public struct NPCEntry : IDataEntry<System.Int32> {
    [JsonProperty]
    public System.String Name { get; set; }

    [JsonIgnore]
    public System.Int32 Id { get; set; }

    [JsonIgnore]
    public System.Boolean ValidEntry => Id > NPCID.NegativeIDCount && Id < NPCLoader.NPCCount;

    [JsonIgnore]
    public System.Boolean VanillaEntry => Id < NPCID.Count;

    private static System.Int32 _uniqueIds = NPCID.NegativeIDCount;

    public NPCEntry(System.String name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public NPCEntry(System.Int32 id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override System.Boolean Equals([NotNullWhen(true)] System.Object obj) {
        return obj is not NPCEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override System.Int32 GetHashCode() {
        return Id;
    }

    public override System.String ToString() {
        return Name;
    }

    public void Initialize() {
        if (!System.String.IsNullOrEmpty(Name)) {
            Id = NPCID.Search.TryGetId(Name, out System.Int32 id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = NPCID.Search.TryGetName(Id, out System.String name) ? name : "Unknown";
        }
    }

    public static implicit operator System.String(NPCEntry entry) {
        return entry.Name;
    }

    public static implicit operator System.Int32(NPCEntry entry) {
        return entry.Id;
    }

    public static explicit operator NPCEntry(System.Int32 id) {
        return new(id);
    }

    public static explicit operator NPCEntry(System.String name) {
        return new(name);
    }

    public static System.Boolean operator ==(NPCEntry left, NPCEntry right) {
        return left.Equals(right);
    }

    public static System.Boolean operator !=(NPCEntry left, NPCEntry right) {
        return !(left == right);
    }
}