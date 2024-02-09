using Aequus.Content.DataSets;

namespace Aequus.Common.Tiles;

public class Slopes : GlobalTile {
    public override bool Slope(int i, int j, int type) {
        Tile tile = Framing.GetTileSafely(i, j - 1);
        return !tile.HasTile || !TileMetadata.PreventsSlopesBelow.Contains(tile.TileType);
    }
}
