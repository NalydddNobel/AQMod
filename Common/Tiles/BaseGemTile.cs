using System.Collections.Generic;
using Terraria.ObjectData;

namespace Aequus.Common.Tiles;

public abstract class BaseGemTile : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
    }

    public override System.Boolean CanPlace(System.Int32 i, System.Int32 j) {
        return TileHelper.GetGemFramingAnchor(i, j).IsSolidTileAnchor();
    }

    public override System.Boolean TileFrame(System.Int32 i, System.Int32 j, ref System.Boolean resetFrame, ref System.Boolean noBreak) {
        Main.tile[i, j].TileFrameX = 0;
        TileHelper.GemFraming(i, j);
        return false;
    }

    public override IEnumerable<Item> GetItemDrops(System.Int32 i, System.Int32 j) {
        System.Int32 item = TileLoader.GetItemDropFromTypeAndStyle(Type, Main.tile[i, j].TileFrameX / 18);
        if (item > 0) {
            return new Item[1] {
                new Item(item)
            };
        }

        return null;
    }

    public TileObjectData GetObjectData(System.Int32 i, System.Int32 j) {
        var tile = Main.tile[i, j];
        System.Int32 style = tile.TileFrameY / 18;
        System.Int32 alt = tile.TileFrameY / 54;
        var objectData = TileObjectData.GetTileData(tile.TileType, style, alt);
        return objectData;
    }
}