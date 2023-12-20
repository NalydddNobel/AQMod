using Aequus.Core.Graphics.Animations;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;

public class SeaPickles2x2 : SeaPicklesTileBase {
    public override void SetStaticDefaults() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        base.SetStaticDefaults();
        TileObjectData.addTile(Type);
    }
}