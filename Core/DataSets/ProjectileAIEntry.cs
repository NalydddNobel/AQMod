using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<ProjectileAIEntry, int>))]
public struct ProjectileAIEntry : IDataEntry<int> {
    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => Id >= 0;

    [JsonIgnore]
    public bool VanillaEntry => Id >= 0;

    private static int _uniqueIds;

    public ProjectileAIEntry(string name) {
        Name = name;
        Initialize();
    }

    public ProjectileAIEntry(int id) {
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not ProjectileAIEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (!string.IsNullOrEmpty(Name)) {
            Id = ProjAIStyleID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = ProjAIStyleID.Search.TryGetName(Id, out string name) ? name : "Unknown";
        }
    }

    public static implicit operator string(ProjectileAIEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(ProjectileAIEntry entry) {
        return entry.Id;
    }

    public static explicit operator ProjectileAIEntry(int id) {
        return new(id);
    }

    public static explicit operator ProjectileAIEntry(string name) {
        return new(name);
    }

    public static bool operator ==(ProjectileAIEntry left, ProjectileAIEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(ProjectileAIEntry left, ProjectileAIEntry right) {
        return !(left == right);
    }
}