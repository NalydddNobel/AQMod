using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Items;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Common.ContentTemplates;

public abstract class UnifiedOre : ModTexturedType, ILocalizedModType, IAddRecipes {
    public readonly ModItem BarItem;
    public readonly ModTile BarTile;

    public readonly ModItem OreItem;
    public readonly ModTile OreTile;

    protected int SortingPriority;

    public int Rarity { get; protected set; }
    public int DustType { get; protected set; }
    public Color Color { get; protected set; }

    public int BarPrice { get; protected set; }

    public int OrePrice { get; protected set; }
    public float OreMiningResist { get; protected set; } = 1f;
    public int OreMiningRequirement { get; protected set; }
    public short OreMetalDetectorPriority { get; protected set; }
    public short OreShineChanceDenominator { get; protected set; }

    public string LocalizationCategory => "Items.Materials";
    public LocalizedText DisplayNameBar => this.GetLocalization("BarDisplayName", () => $"{PrettyPrintName()} Bar");
    public LocalizedText TooltipBar => this.GetLocalization("BarTooltip", () => "");
    public LocalizedText DisplayNameOre => this.GetLocalization("OreDisplayName", () => $"{PrettyPrintName()} Ore");
    public LocalizedText TooltipOre => this.GetLocalization("OreTooltip", () => "");

    public UnifiedOre() {
        BarTile = new InstancedBarTile(this);
        BarItem = new InstancedBarItem(this);
        OreTile = new InstancedOreTile(this);
        OreItem = new InstancedOreItem(this);
    }

    public override void Load() {
        this.RegisterMembers();
    }
    protected override void Register() {
    }

    public override void SetupContent() {
        Main.tileOreFinderPriority[OreTile.Type] = OreMetalDetectorPriority;
        SetStaticDefaults();
    }

    protected virtual void AddRecipes() {

    }
    void IAddRecipes.AddRecipes(Aequus aequus) {
        AddRecipes();
    }
}

internal class InstancedBarItem(UnifiedOre Parent) : InstancedModItem($"{Parent.Name}Bar", $"{Parent.Texture}Bar") {
    public override LocalizedText DisplayName => Parent.DisplayNameBar;
    public override LocalizedText Tooltip => Parent.TooltipBar;
    public override string LocalizationCategory => Parent.LocalizationCategory;

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.CopperBar);
        Item.rare = Parent.Rarity;
        Item.value = Parent.BarPrice;
        Item.createTile = Parent.BarTile.Type;
        Item.placeStyle = 0;
    }
}

internal class InstancedBarTile(UnifiedOre Parent) : InstancedModTile($"{Parent.Name}Bar", $"{Parent.Texture}BarTile") {
    public override void SetStaticDefaults() {
        Main.tileShine[Type] = 1100;
        Main.tileSolid[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);

        AddMapEntry(Parent.Color, Language.GetText("MapObject.MetalBar"));
    }
}

internal class InstancedOreTile(UnifiedOre Parent) : InstancedModTile($"{Parent.Name}Ore", $"{Parent.Texture}Ore") {
    public override string LocalizationCategory => Parent.LocalizationCategory;

    public override void SetStaticDefaults() {
        TileID.Sets.Ore[Type] = true;
        Main.tileSpelunker[Type] = true;
        Main.tileOreFinderPriority[Type] = Parent.OreMetalDetectorPriority;
        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = Parent.OreShineChanceDenominator;
        Main.tileMergeDirt[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        AddMapEntry(Parent.Color, Parent.DisplayNameOre);

        DustType = Parent.DustType;
        HitSound = SoundID.Tink;
        MineResist = Parent.OreMiningResist;
        MinPick = Parent.OreMiningRequirement;
    }
}

internal class InstancedOreItem(UnifiedOre Parent) : InstancedTileItem(Parent.OreTile, Settings: new() {
    Rare = Parent.Rarity,
    Value = Parent.OrePrice,
}) {
    public override LocalizedText DisplayName => Parent.DisplayNameOre;
    public override LocalizedText Tooltip => Parent.TooltipOre;
    public override string Texture => Parent.OreTile.Texture;
    public override string LocalizationCategory => Parent.LocalizationCategory;

    public override void SetStaticDefaults() {
        Main.RegisterItemAnimation(Type, new CustomItemDrawFrame(new Rectangle(256, 96, 16, 16)));
    }
}