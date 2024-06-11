using Aequus.DataSets.Json;
using Newtonsoft.Json;
using System.Reflection;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.DataSets.Structures;

public abstract class DataSet : IModType, ILoad, ISetupContent, IPostSetupContent, IAddRecipes, IPostAddRecipes, IJsonHolder {
    private readonly BindingFlags _memberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    [JsonIgnore]
    protected EmbeddedJsonFile File { get; private set; }

    [JsonIgnore]
    public virtual string FilePath => Name.Replace("DataSet", "");

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

    public virtual void Load() {
    }
    public virtual void SetupContent() {
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

    public void SetupContent(Mod mod) {
        SetupContent();
    }

    public void PostSetupContent(Mod mod) {
        File.Apply();
        PostSetupContent();
    }

    public void AddRecipes(Mod mod) {
        AddRecipes();
    }

    public void PostAddRecipes(Mod mod) {
        PostAddRecipes();
        File.GenerateEmbeddedFiles();
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