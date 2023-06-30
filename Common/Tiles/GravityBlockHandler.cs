using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles {
    public class GravityBlockHandler {
        public static sbyte MaximumReach = 24;

        public static sbyte CheckGravityBlocks(Vector2 position, int width, int height) {
            int x = (int)((position.X + width / 2f) / 16f);
            int y = (int)((position.Y + height / 2f) / 16f);
            if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x, y - MaximumReach) || !WorldGen.InWorld(x, y + MaximumReach)
                || !TileHelper.IsSectionLoaded(x, y) || !TileHelper.IsSectionLoaded(x, y - MaximumReach) || !TileHelper.IsSectionLoaded(x, y + MaximumReach)) {
                return 0;
            }

            for (int j = MaximumReach; j > -MaximumReach; j--) {
                var tile = Framing.GetTileSafely(x, y + j);
                if (tile.HasTile && TileLoader.GetTile(tile.TileType) is IGravityBlock gravityBlock) {
                    var gravity = gravityBlock.GravityType;
                    if (Math.Sign(j) != Math.Sign(gravity)) {
                        continue;
                    }
                    int reach = Math.Min(gravityBlock.GetReach(x, y + j), MaximumReach);
                    if (reach < Math.Abs(j)) {
                        continue;
                    }
                    return gravity;
                }
            }
            return 0;
        }
    }
}