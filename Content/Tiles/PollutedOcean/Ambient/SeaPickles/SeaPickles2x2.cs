using Aequus.Content.Tiles.PollutedOcean.PolymerSands;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.SeaPickles;

internal class SeaPickles2x2 : SeaPicklesTileBase {
    public SeaPickles2x2() : base() { }
    public SeaPickles2x2(string name, bool natural) : base(name, natural) { }

    public override int UseItem => Instance<PolymerSand>().Item.Type;

    public override int[] Styles => [0, 1, 2];

    public override FlexibleTileWand Size => FlexibleTileWand.RubblePlacementLarge;

    public override void SafeSetStaticDefaults() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        base.SafeSetStaticDefaults();
        TileObjectData.addTile(Type);
    }
}