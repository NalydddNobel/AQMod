using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Old.Common.Carpentry {
    public interface IScanInfo {
        Rectangle Area { get; }
        public Tile this[int x, int y] {
            get => Framing.GetTileSafely(x + Area.X, y + Area.Y);
        }
    }
}
