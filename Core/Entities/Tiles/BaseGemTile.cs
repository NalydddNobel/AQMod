using System.Collections.Generic;

namespace AequusRemake.Core.Entities.Tiles;

public abstract class BaseGemTile : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
    }

    public override bool CanPlace(int i, int j) {
        TileAnchorDirection anchor = TileHelper.GetGemFramingAnchor(i, j);
        return anchor.IsSolidTileAnchor();
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        Main.tile[i, j].TileFrameX = 0;
        TileHelper.GemFraming(i, j);
        return false;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        int item = TileLoader.GetItemDropFromTypeAndStyle(Type, Main.tile[i, j].TileFrameX / 18);
        if (item > 0) {
            return new Item[1] {
                new Item(item)
            };
        }

        return null;
    }
}