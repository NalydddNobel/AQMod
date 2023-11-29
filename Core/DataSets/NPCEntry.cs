using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Terraria.ID;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<NPCEntry, int>))]
public struct NPCEntry : IDataEntry<int> {
    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => Id <= NPCID.NegativeIDCount;

    [JsonIgnore]
    public bool VanillaEntry => Id < NPCID.Count;

    private static int _uniqueIds;

    public NPCEntry(string name) {
        Name = name;
        Initialize();
    }

    public NPCEntry(int id) {
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not NPCEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (!string.IsNullOrEmpty(Name)) {
            Id = NPCID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = NPCID.Search.TryGetName(Id, out string name) ? name : "Unknown";
        }
    }

    public static implicit operator string(NPCEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(NPCEntry entry) {
        return entry.Id;
    }

    public static explicit operator NPCEntry(int id) {
        return new(id);
    }

    public static explicit operator NPCEntry(string name) {
        return new(name);
    }

    public static bool operator ==(NPCEntry left, NPCEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(NPCEntry left, NPCEntry right) {
        return !(left == right);
    }
}