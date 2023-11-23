using Aequus.Core.Autoloading;
using Newtonsoft.Json;
using ReLogic.Utilities;
using System;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Core.DataSets; 

public abstract class DataSet : IModType, ILoadable, IPostSetupContent, IAddRecipes, IPostAddRecipes {
    private readonly BindingFlags _memberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    protected MetadataFileLoader File { get; private set; }
    [JsonIgnore]
    public virtual string FilePath => this.GetFilePath();

    [JsonIgnore]
    public Mod Mod { get; set; }
    [JsonIgnore]
    public virtual string Name => GetType().Name;
    [JsonIgnore]
    public string FullName => Mod.Name + "/" + Name;

    [JsonIgnore]
    public FieldInfo[] _fields;

    public DataSet() {
        var type = GetType();
        _fields = type.GetFields(_memberBindingFlags);
        var typeIdAttribute = type.GetAttribute<DataIDAttribute>();
        if (typeIdAttribute != null) {
            var baseIdDictionary = typeIdAttribute.GetIdDictionary();
            foreach (var f in _fields) {
                if (f.IsInitOnly) {
                    continue;
                }

                var fieldAttribute = f.GetCustomAttribute<DataIDAttribute>();

                var idDictionary = fieldAttribute != null ? fieldAttribute.GetIdDictionary() : baseIdDictionary;

                if (f.FieldType == typeof(DataIDValueSet)) {
                    f.SetValue(this, new DataIDValueSet(idDictionary));
                }
                else if (f.FieldType == typeof(DataIDBoolArraySet)) {
                    f.SetValue(this, new DataIDBoolArraySet(idDictionary));
                }
                else if (f.FieldType == typeof(DataIDKeyValueDictionary)) {
                    var keyValueAttribute = f.GetCustomAttribute<DataIDKeyValueAttribute>();
                    if (keyValueAttribute != null) {
                        f.SetValue(this, new DataIDKeyValueDictionary(keyValueAttribute.GetKeyIdDictionary(), keyValueAttribute.GetValueIdDictionary()));
                    }
                    else {
                        f.SetValue(this, new DataIDKeyValueDictionary(idDictionary, idDictionary));
                    }
                }
                else if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition().IsAssignableFrom(typeof(DataIDKeyDictionary<>))) {
                    f.SetValue(this, Activator.CreateInstance(f.FieldType, idDictionary));
                }
            }
        }
    }

    public void Load(Mod mod) {
        Mod = mod;
        File = new(this);
        Load();
    }
    public virtual void Load() {
    }

    public void Unload() {
        OnUnload();

        // Automatically clear data sets
        if (_fields != null) {
            foreach (var f in _fields) {
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
    public virtual void OnUnload() {
    }

    public void PostSetupContent(Aequus aequus) {
        File.ApplyToDataSet();
        PostSetupContent();
    }
    public virtual void PostSetupContent() {
    }

    public void AddRecipes(Aequus aequus) {
        AddRecipes();
    }
    public virtual void AddRecipes() {
    }

    public void PostAddRecipes(Aequus aequus) {
        PostAddRecipes();
#if DEBUG
        File.CreateTempFile();
#endif
    }
    public virtual void PostAddRecipes() {
    }
}