using Aequus.Core.Autoloading;
using Newtonsoft.Json;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Core.DataSets;

public abstract class DataSet : IModType, ILoadable, IPostSetupContent, IAddRecipes, IPostAddRecipes {
    private readonly BindingFlags _memberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    [JsonIgnore]
    protected MetadataFileLoader File { get; private set; }

    [JsonIgnore]
    public virtual string FilePath => $"{Mod.Name}/Assets/Metadata/{Name}";

    [JsonIgnore]
    public Mod Mod { get; set; }
    [JsonIgnore]
    public virtual string Name => GetType().Name;
    [JsonIgnore]
    public string FullName => Mod.Name + "/" + Name;

    [JsonIgnore]
    public FieldInfo[] _fields;

    public DataSet() {
        _fields = GetType().GetFields(_memberBindingFlags);
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