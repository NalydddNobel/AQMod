using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<NPCAIEntry, int>))]
public struct NPCAIEntry : IDataEntry<int> {
    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => Id >= 0;

    [JsonIgnore]
    public bool VanillaEntry => Id >= 0;

    private static int _uniqueIds = -1;

    public NPCAIEntry(string name) {
        Name = name;
        Initialize();
    }

    public NPCAIEntry(int id) {
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not NPCAIEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (!string.IsNullOrEmpty(Name)) {
            Id = NPCAIStyleID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = NPCAIStyleID.Search.TryGetName(Id, out string name) ? name : "Unknown";
        }
    }

    public static implicit operator string(NPCAIEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(NPCAIEntry entry) {
        return entry.Id;
    }

    public static explicit operator NPCAIEntry(int id) {
        return new(id);
    }

    public static explicit operator NPCAIEntry(string name) {
        return new(name);
    }

    public static bool operator ==(NPCAIEntry left, NPCAIEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(NPCAIEntry left, NPCAIEntry right) {
        return !(left == right);
    }
}