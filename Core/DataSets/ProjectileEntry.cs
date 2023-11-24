using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Terraria.ID;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<ProjectileEntry, int>))]
public struct ProjectileEntry : IDataEntry<int> {
    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => Id > ProjectileID.None;

    [JsonIgnore]
    public bool VanillaEntry => Id < ProjectileID.Count;

    private static int _uniqueIds;

    public ProjectileEntry(string name) {
        Name = name;
        Initialize();
    }

    public ProjectileEntry(int id) {
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not ProjectileEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (!string.IsNullOrEmpty(Name)) {
            Id = ProjectileID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = ProjectileID.Search.TryGetName(Id, out string name) ? name : "Unknown";
        }
    }

    public static implicit operator string(ProjectileEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(ProjectileEntry entry) {
        return entry.Id;
    }

    public static explicit operator ProjectileEntry(int id) {
        return new(id);
    }

    public static explicit operator ProjectileEntry(string name) {
        return new(name);
    }

    public static bool operator ==(ProjectileEntry left, ProjectileEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(ProjectileEntry left, ProjectileEntry right) {
        return !(left == right);
    }
}