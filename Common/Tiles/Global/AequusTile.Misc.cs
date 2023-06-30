using Aequus.Common.DataSets;
using Terraria;

namespace Aequus {
    public partial class AequusTile {
        public override bool Slope(int i, int j, int type) {
            var aboveTile = Framing.GetTileSafely(i, j - 1);
            if (!aboveTile.HasTile) {
                return true;
            }
            if (TileSets.PreventsSlopesBelow.Contains(aboveTile.TileType)) {
                return false;
            }
            return true;
        }
    }
}