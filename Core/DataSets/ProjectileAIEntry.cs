using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<ProjectileAIEntry, System.Int32>))]
public struct ProjectileAIEntry : IDataEntry<System.Int32> {
    [JsonProperty]
    public System.String Name { get; set; }

    [JsonIgnore]
    public System.Int32 Id { get; set; }

    [JsonIgnore]
    public System.Boolean ValidEntry => Id >= 0;

    [JsonIgnore]
    public System.Boolean VanillaEntry => Id >= 0;

    private static System.Int32 _uniqueIds;

    public ProjectileAIEntry(System.String name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public ProjectileAIEntry(System.Int32 id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override System.Boolean Equals([NotNullWhen(true)] System.Object obj) {
        return obj is not ProjectileAIEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override System.Int32 GetHashCode() {
        return Id;
    }

    public override System.String ToString() {
        return Name;
    }

    public void Initialize() {
        if (!System.String.IsNullOrEmpty(Name)) {
            Id = ProjAIStyleID.Search.TryGetId(Name, out System.Int32 id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = ProjAIStyleID.Search.TryGetName(Id, out System.String name) ? name : "Unknown";
        }
    }

    public static implicit operator System.String(ProjectileAIEntry entry) {
        return entry.Name;
    }

    public static implicit operator System.Int32(ProjectileAIEntry entry) {
        return entry.Id;
    }

    public static explicit operator ProjectileAIEntry(System.Int32 id) {
        return new(id);
    }

    public static explicit operator ProjectileAIEntry(System.String name) {
        return new(name);
    }

    public static System.Boolean operator ==(ProjectileAIEntry left, ProjectileAIEntry right) {
        return left.Equals(right);
    }

    public static System.Boolean operator !=(ProjectileAIEntry left, ProjectileAIEntry right) {
        return !(left == right);
    }
}