﻿using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

[JsonConverter(typeof(DataEntryConverter<ProjectileEntry, System.Int32>))]
public struct ProjectileEntry : IDataEntry<System.Int32> {
    [JsonProperty]
    public System.String Name { get; set; }

    [JsonIgnore]
    public System.Int32 Id { get; set; }

    [JsonIgnore]
    public System.Boolean ValidEntry => Id > ProjectileID.None;

    [JsonIgnore]
    public System.Boolean VanillaEntry => Id < ProjectileID.Count;

    private static System.Int32 _uniqueIds;

    public ProjectileEntry(System.String name) {
        Name = name;
        Id = 0;
        Initialize();
    }

    public ProjectileEntry(System.Int32 id) {
        Name = null;
        Id = id;
        Initialize();
    }

    public override System.Boolean Equals([NotNullWhen(true)] System.Object obj) {
        return obj is not ProjectileEntry otherEntry ? false : otherEntry.Id.Equals(Id);
    }

    public override System.Int32 GetHashCode() {
        return Id;
    }

    public override System.String ToString() {
        return Name;
    }

    public void Initialize() {
        if (!System.String.IsNullOrEmpty(Name)) {
            Id = ProjectileID.Search.TryGetId(Name, out System.Int32 id) ? id : _uniqueIds--;
        }
        else if (ValidEntry) {
            Name = ProjectileID.Search.TryGetName(Id, out System.String name) ? name : "Unknown";
        }
    }

    public static implicit operator System.String(ProjectileEntry entry) {
        return entry.Name;
    }

    public static implicit operator System.Int32(ProjectileEntry entry) {
        return entry.Id;
    }

    public static explicit operator ProjectileEntry(System.Int32 id) {
        return new(id);
    }

    public static explicit operator ProjectileEntry(System.String name) {
        return new(name);
    }

    public static System.Boolean operator ==(ProjectileEntry left, ProjectileEntry right) {
        return left.Equals(right);
    }

    public static System.Boolean operator !=(ProjectileEntry left, ProjectileEntry right) {
        return !(left == right);
    }
}