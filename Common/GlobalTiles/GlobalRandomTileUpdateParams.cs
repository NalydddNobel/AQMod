using Terraria;

namespace Aequus.Common.GlobalTiles {
    public readonly struct GlobalRandomTileUpdateParams {
        public readonly int X;
        public readonly int Y;
        public readonly int TileTypeCache;
        public readonly int WallTypeCache;
        public readonly Tile Tile;
        public readonly int GemX;
        public readonly int GemY;
        public readonly Tile GemTile;

        public GlobalRandomTileUpdateParams(int x, int y, int tileTypeCache, int wallTypeCache) {
            X = x;
            Y = y;
            TileTypeCache = tileTypeCache;
            WallTypeCache = wallTypeCache;
            Tile = Main.tile[x, y];
            GemX = x + WorldGen.genRand.Next(-1, 2);
            GemY = y + WorldGen.genRand.Next(-1, 2);
            GemTile = Main.tile[GemX, GemY];
        }
    }
}