using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<PrefixEntry, int>))]
public struct PrefixEntry : IDataEntry<int> {
    private const string StringNone = "None";
    private const string StringRandom = "Random";
    private const int IdNone = 0;
    private const int IdRandom = -1;

    [JsonProperty]
    public string Name { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool ValidEntry => true;

    [JsonIgnore]
    public bool VanillaEntry => Id < PrefixID.Count;

    private static int _uniqueIds;

    public PrefixEntry(string name) {
        Name = name;
        Id = -1000;
        Initialize();
    }

    public PrefixEntry(int id) {
        Id = id;
        Initialize();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        return obj is not PrefixEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override int GetHashCode() {
        return Id;
    }

    public override string ToString() {
        return Name;
    }

    public void Initialize() {
        if (Id == -1000) {
            if (!string.IsNullOrEmpty(Name)) {
                Id = Name switch {
                    StringRandom => IdRandom,
                    StringNone => IdNone,
                    _ => PrefixID.Search.TryGetId(Name, out int id) ? id : _uniqueIds--,
                };
            }
        }

        Name = Id switch {
            IdRandom => StringRandom,
            IdNone => StringNone,
            _ => PrefixID.Search.TryGetName(Id, out string name) ? name : "Unknown",
        };
    }

    public static implicit operator string(PrefixEntry entry) {
        return entry.Name;
    }

    public static implicit operator int(PrefixEntry entry) {
        return entry.Id;
    }

    public static explicit operator PrefixEntry(int id) {
        return new(id);
    }

    public static explicit operator PrefixEntry(string name) {
        return new(name);
    }

    public static bool operator ==(PrefixEntry left, PrefixEntry right) {
        return left.Equals(right);
    }

    public static bool operator !=(PrefixEntry left, PrefixEntry right) {
        return !(left == right);
    }
}