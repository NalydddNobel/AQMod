using AequusRemake.Core.Structures.Pooling;
using AequusRemake.DataSets.Json;
using Terraria.Localization;

namespace AequusRemake.Systems.Elements;

public abstract partial class Element(Color color) : ModTexturedType, ILocalizedModType {
    public string LocalizationCategory => "Elements";

    public LocalizedText DisplayName { get; private set; }

    public override string Name => base.Name.Replace("Element", "");

    public ISpriteProvider Sprite { get; set; }

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
        SetStaticDefaults();
    }

    internal void OnPostSetupRecipes() {
        SetupRelations();
        _file.GenerateEmbeddedFiles();
    }
}

public abstract class VanillaElement(int Frame, Color color) : Element(color) {
    public const int AirFrame = 0;
    public const int EarthFrame = 1;
    public const int WaterFrame = 2;
    public const int FlameFrame = 3;
    public const int FrostFrame = 4;
    public const int LightFrame = 5;
    public const int ShadowFrame = 6;

    public override void SetStaticDefaults() {
        Sprite = new FramedSpriteProvider(AequusTextures.ElementIcons, horizontalFrames: 7, frameX: Frame, verticalFrames: 2, frameY: 0);
    }
}