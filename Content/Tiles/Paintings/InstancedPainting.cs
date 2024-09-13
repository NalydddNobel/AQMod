using Aequus.Common.ContentTemplates.Generic;
using System;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Paintings;

internal class InstancedPainting : InstancedModTile, IPainting {
    public readonly string ItemTexture;
    public readonly int Rare;
    public readonly int Value;
    public readonly Color? MapColor;
    public readonly Lazy<LocalizedText>? TileName;
    public readonly ModItem Item;

    public int Width { get; init; }
    public int Height { get; init; }

    ushort IPainting.TileType => Type;

    int IPainting.ItemType => Item.Type;

    Mod IPainting.Mod => Mod;

    public InstancedPainting(string name, string texture, string ItemTexture, int W, int H, int Rare, int Value, LocalizedText? Tooltip, Color? MapColor = null, Lazy<LocalizedText>? TileName = null) : base($"{name}Painting", texture) {
        Width = W;
        Height = H;
        this.ItemTexture = ItemTexture;
        this.Rare = Rare;
        this.Value = Value;
        this.MapColor = MapColor;
        this.TileName = TileName;
        Item = new InstancedPaintingItem(TileName, Tooltip, this);
    }

    public override string LocalizationCategory => "Tiles.Paintings";

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        new PaintingInitInfo(Width, Height)
            .SetupPainting(this, TileObjectData.newTile);
        TileObjectData.addTile(Type);

        DustType = DustID.WoodFurniture;
        Paintings p = ModContent.GetInstance<Paintings>();
        AddMapEntry(MapColor ?? p.DefaultPaintingColor, TileName?.Value ?? p.DefaultPaintingName);
    }

    //public override IEnumerable<Item> GetItemDrops(int i, int j) {
    //    yield return new Item(Item.Type);
    //}
}
