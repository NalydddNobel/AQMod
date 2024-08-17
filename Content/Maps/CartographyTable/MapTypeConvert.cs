using Aequus.Common;
using System.Collections.Generic;
using Terraria.Map;

namespace Aequus.Content.Maps.CartographyTable;

public class MapTypeConvert : LoadedType, IPostAddRecipes {
    public static MapTypeConvert Instance => ModContent.GetInstance<MapTypeConvert>();

    public const byte None = 0;
    public const byte Wall = 1;
    public const byte Block = 2;
    public const byte BGObject = 3;

    private readonly Dictionary<byte, ushort> CompressedIdToMapTileId = [];

    void IPostAddRecipes.PostAddRecipes(Aequus aequus) {
        if (Main.dedServ) {
            return;
        }

        ushort baseTile = ModContent.GetInstance<CartographyTable>().Type;
        CompressedIdToMapTileId[None] = (ushort)MapHelper.TileToLookup(baseTile, 1);
        CompressedIdToMapTileId[Wall] = (ushort)MapHelper.TileToLookup(baseTile, 2);
        CompressedIdToMapTileId[Block] = (ushort)MapHelper.TileToLookup(baseTile, 4);
        CompressedIdToMapTileId[BGObject] = (ushort)MapHelper.TileToLookup(baseTile, 3);
    }

    public byte ToServerType(Tile tile, ushort clientMapTileType) {
        /*
        if (MapTileIdToCompressed.TryGetValue(mapTileType, out byte compressed)) {
            return compressed;
        }
        */

        // Check if this is a block.
        if (!tile.IsTileInvisible && tile.HasTile) {
            if (!Main.tileFrameImportant[tile.TileType] && tile.IsSolid()) {
                return Block;
            }

            return BGObject;
        }

        // Check if this is a wall.
        if (tile.WallType > 0 && !tile.IsWallInvisible) {
            return Wall;
        }

        return None;
    }

    public ushort ToClientType(byte serverType) {
        if (CompressedIdToMapTileId.TryGetValue(serverType, out ushort clientType)) {
            return clientType;
        }

        return 0;
    }
}