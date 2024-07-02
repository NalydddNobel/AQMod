using Aequus.DataSets;

namespace Aequus.Core.Entities.Tiles;

public class Slopes : GlobalTile {
    public override bool Slope(int i, int j, int type) {
        Tile tile = Framing.GetTileSafely(i, j - 1);
        return !tile.HasTile || !TileDataSet.PreventsSlopesBelow.Contains(tile.TileType);
    }
}
