using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;

[LegacyName("SeaPickle", "SeaPickleTile")]
public class SeaPickles1x1 : SeaPicklesTileBase {
    public override void SetStaticDefaults() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        base.SetStaticDefaults();
        TileObjectData.addTile(Type);

        LightMagnitudeMultiplier = 0.5f;
    }
}