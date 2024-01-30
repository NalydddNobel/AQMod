using Aequus.Content.DataSets;

namespace Aequus.Common.Tiles;

public class Slopes : GlobalTile {
    public override System.Boolean Slope(System.Int32 i, System.Int32 j, System.Int32 type) {
        Tile tile = Framing.GetTileSafely(i, j - 1);
        return !tile.HasTile || !TileSets.PreventsSlopesBelow.Contains(tile.TileType);
    }
}
