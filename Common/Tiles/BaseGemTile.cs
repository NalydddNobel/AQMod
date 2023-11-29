using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Common.Tiles;

public abstract class BaseGemTile : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
    }

    public override bool CanPlace(int i, int j) {
        return TileHelper.GetGemFramingAnchor(i, j).IsSolidTileAnchor();
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

    public TileObjectData GetObjectData(int i, int j) {
        var tile = Main.tile[i, j];
        int style = tile.TileFrameY / 18;
        int alt = tile.TileFrameY / 54;
        var objectData = TileObjectData.GetTileData(tile.TileType, style, alt);
        return objectData;
    }
}