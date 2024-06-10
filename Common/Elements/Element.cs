using Terraria.Localization;

namespace Aequus.Common.Elements;

public abstract partial class Element(Color color) : ModTexturedType, ILocalizedModType {
    public string LocalizationCategory => "Elements";

    public LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);

    public override string Name => base.Name.Replace("Element", "");

    public int Type { get; internal set; }

    public readonly Color Color = color;

    protected sealed override void Register() {
        ElementLoader.RegisterElement(this);
        ModItem mask = new InstancedElementalHat(this);
        Mod.AddContent(mask);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
        _ = DisplayName;
    }

    public abstract void SetupRelations();
}
