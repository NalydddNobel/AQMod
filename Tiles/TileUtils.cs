using Terraria;

namespace AQMod.Tiles
{
    internal static class TileUtils
    {
        public static bool SolidTop(this Tile tile)
        {
            return Main.tileSolidTop[tile.type];
        }

        /// <summary>
        /// Returns whether this tile is actually a solid top using its frames
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static bool IsASolidTop(this Tile tile)
        {
            if (Main.tileFrameImportant[tile.type])
            {
                return tile.frameY == 0 && Main.tileSolidTop[tile.type]; // TODO: actually get the code for checking if a tile has a solid top collision.
                                         // Since this will break with any tile which has a solid top, and has styles on the Y direction!
            }
            return Main.tileSolidTop[tile.type];
        }

        public static bool Solid(this Tile tile)
        {
            return Main.tileSolid[tile.type];
        }
    }
}
