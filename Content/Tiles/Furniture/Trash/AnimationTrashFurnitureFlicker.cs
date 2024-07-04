using AequusRemake.Core.Graphics.Animations;

namespace AequusRemake.Content.Tiles.Furniture.Trash;

public class AnimationTrashFurnitureFlicker(int AnchorTileType) : ITileAnimation {
    public int NoLight;

    public bool Update(int x, int y) {
        Tile tile = Main.tile[x, y];

        int flickerChance = 720;
#if DEBUG
        if (Main.mouseRight) {
            flickerChance = 240;
        }
#endif

        if (NoLight == 0 && Main.rand.NextBool(flickerChance)) {
            NoLight = Main.rand.Next(20);
        }
        else if (NoLight > 0) {
            NoLight--;
        }

        return tile.HasTile && tile.TileType == AnchorTileType && Cull2D.Tile(x, y);
    }
}
