using Aequus.Common.Entities.Tiles;

namespace Aequus.Tiles.MossCaves.Radon;
public class RadonMossBrickTile : RadonMossTile {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        TileID.Sets.tileMossBrick[Type] = true;
        Main.tileMoss[Type] = false;
        RegisterItemDrop(ItemID.GrayBrick);
    }

    public override bool? ModifyPlaceTile(ref PlaceTileInfo info) {
        return null;
    }
}