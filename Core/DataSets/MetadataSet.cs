using Aequus.Core.Initialization;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aequus.Core.DataSets;

public abstract class MetadataSet : IModType, ILoad, ISetStaticDefaults,  IPostSetupContent, IAddRecipes, IPostAddRecipes {
    private readonly BindingFlags _memberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    [JsonIgnore]
    protected MetadataFile File { get; private set; }

    [JsonIgnore]
    public virtual string FilePath => $"{Mod.Name}/Assets/Metadata/{Name.Replace("Metadata", "")}";

    [JsonIgnore]
    public Mod Mod { get; set; }
    [JsonIgnore]
    public virtual string Name => GetType().Name;
    [JsonIgnore]
    public string FullName => Mod.Name + "/" + Name;

    [JsonIgnore]
    public FieldInfo[] _fields;

    public MetadataSet() {
        _fields = GetType().GetFields(_memberBindingFlags);
    }

    public virtual void Load() {
    }
    public virtual void SetStaticDefaults() {
    }
    public virtual void PostSetupContent() {
    }
    public virtual void AddRecipes() {
    }
    public virtual void PostAddRecipes() {
    }
    public virtual void OnUnload() {
    }

    public void Load(Mod mod) {
        Mod = mod;
        File = new(this);
        Load();
    }

    public void SetStaticDefaults(Aequus aequus) {
        SetStaticDefaults();
    }

    public void PostSetupContent(Aequus aequus) {
        File.Apply();
        PostSetupContent();
    }

    public void AddRecipes(Aequus aequus) {
        AddRecipes();
    }

    public void PostAddRecipes(Aequus aequus) {
        PostAddRecipes();
        File.Gen();
    }

    public void Unload() {
        OnUnload();

        // Automatically clear data sets
        if (_fields != null) {
            foreach (FieldInfo f in _fields) {
                InvokeClear(f.GetValue(this));
            }
            _fields = null;
        }

        static void InvokeClear(object obj) {
            if (obj == null) {
                return;
            }

            var m = obj.GetType().GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);
            if (m == null) {
                return;
            }

            var p = m.GetParameters();
            if (p.Length > 0) {
                return;
            }

            m.Invoke(obj, null);
        }
    }
}