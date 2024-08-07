using Aequus.Common.ContentGeneration;
using Aequus.Common.Utilities.Helpers;
using System;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Paintings;

public class Paintings : ILoadable {
    public readonly Color DefaultPaintingColor = new Color(120, 85, 60);
    public readonly LocalizedText DefaultPaintingName = Language.GetText("MapObject.Painting");

    void ILoadable.Load(Mod mod) {
        mod.AddContent(new InstancedPainting("Rockman", AequusTextures.RockmanPainting.FullPath, AequusTextures.RockmanPaintingItem.FullPath, 2, 3,
            ItemRarityID.White, Item.sellPrice(silver: 10), Color.Gray, new Lazy<LocalizedText>(() => ModContent.GetInstance<global::Aequus.Items.Weapons.Melee.Swords.RockMan.RockMan>().DisplayName)));
    }

    void ILoadable.Unload() {
    }
}

internal class InstancedPainting : InstancedModTile {
    public readonly string ItemTexture;
    public readonly int Rare;
    public readonly int Value;
    public readonly int Width;
    public readonly int Height;
    public readonly Color? MapColor;
    public readonly Lazy<LocalizedText>? TileName;
    public readonly ModItem Item;

    public InstancedPainting(string name, string texture, string ItemTexture, int W, int H, int Rare, int Value, Color? MapColor = null, Lazy<LocalizedText>? TileName = null) : base($"{name}Painting", texture) {
        Width = W;
        Height = H;
        this.ItemTexture = ItemTexture;
        this.Rare = Rare;
        this.Value = Value;
        Item = new InstancedPaintingItem(TileName, this);
    }

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.Width = Width;
        TileObjectData.newTile.Height = Height;
        TileObjectData.newTile.CoordinateHeights = ArrayTools.New(Height, (i) => 16);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 36;
        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        Paintings p = ModContent.GetInstance<Paintings>();
        AddMapEntry(MapColor ?? p.DefaultPaintingColor, TileName?.Value ?? p.DefaultPaintingName);
    }
}

internal class InstancedPaintingItem(Lazy<LocalizedText>? OverrideName, InstancedPainting Parent) : InstancedTileItem(Parent, 0, rarity: Parent.Rare, value: Parent.Value) {
    public override string Texture => Parent.ItemTexture;

    public override LocalizedText DisplayName => OverrideName?.Value ?? base.DisplayName;
    public override LocalizedText Tooltip => LocalizedText.Empty;
}
