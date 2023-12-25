using Aequus.Content.Biomes.PollutedOcean.Tiles.PolymerSands;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;

internal class SeaPickles2x2 : SeaPicklesTileBase {
    public SeaPickles2x2() : base() { }
    public SeaPickles2x2(string name, string texture, bool natural) : base(name, texture, natural) { }

    public override int UseItem => PolymerSand.Item.Type;

    public override int[] Styles => new[] { 0, 1, 2 };

    public override FlexibleTileWand Size => FlexibleTileWand.RubblePlacementLarge;

    public override void SafeSetStaticDefaults() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        base.SafeSetStaticDefaults();
        TileObjectData.addTile(Type);
    }
}