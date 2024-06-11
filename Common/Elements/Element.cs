using Aequus.DataSets.Json;
using ReLogic.Content;
using Terraria.Localization;

namespace Aequus.Common.Elements;

public abstract partial class Element(Color color) : ModTexturedType, ILocalizedModType {
    public string LocalizationCategory => "Elements";

    public LocalizedText DisplayName { get; private set; }

    public override string Name => base.Name.Replace("Element", "");

    internal Asset<Texture2D> texture { get; private set; }

    public int Type { get; internal set; }

    protected EmbeddedJsonFile _file;

    public readonly Color Color = color;

    protected abstract void SetupRelations();

    protected sealed override void Register() {
        ElementLoader.RegisterElement(this);
        _file = new(new JSON(this));
        DisplayName = this.GetLocalization("DisplayName", PrettyPrintName);

#if DEBUG
        ModItem mask = new InstancedElementalHat(this);
        Mod.AddContent(mask);
#endif
    }

    public sealed override void SetupContent() {
        texture = ModContent.Request<Texture2D>(Texture);
        SetStaticDefaults();
    }

    internal void OnPostSetupRecipes() {
        SetupRelations();
        _file.GenerateEmbeddedFiles();
    }
}
