using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;

public class SeaPickles1x2 : SeaPicklesTileBase {
    public override void SetStaticDefaults() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        base.SetStaticDefaults();
        TileObjectData.addTile(Type);
    }
}