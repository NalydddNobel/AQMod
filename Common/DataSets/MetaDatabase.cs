using Newtonsoft.Json;
using System.IO;

namespace Aequus.Common.DataSets;

[JsonObject(MemberSerialization.OptIn)]
public abstract class MetaDatabase : LoadedType, IJsonHolder {
    string IJsonHolder.FilePath => Path.Join(Category, Name);

    protected EmbeddedJsonFile? File { get; private set; }

    public abstract string Category { get; }

    public virtual string Name => GetType().Name.Replace("Database", "");

    protected override void Load() {
        File = new(this);
    }

    public override void PostSetupContent() {
        File!.Apply();
    }

    public override void PostAddRecipes() {
        File!.GenerateEmbeddedFiles();
    }
}
