using AequusRemake.Content.Tiles.PollutedOcean.PolymerSands;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace AequusRemake.Content.Tiles.PollutedOcean.Ambient.SeaPickles;

internal class SeaPickles1x2 : SeaPicklesTileBase {
    public SeaPickles1x2() : base() { }
    public SeaPickles1x2(string name, string texture, bool natural) : base(name, texture, natural) { }

    public override int UseItem => PolymerSand.Item.Type;

    public override int[] Styles => new[] { 0, 1, 2 };

    public override FlexibleTileWand Size => FlexibleTileWand.RubblePlacementMedium;

    public override void SafeSetStaticDefaults() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        base.SafeSetStaticDefaults();
        TileObjectData.addTile(Type);
    }
}