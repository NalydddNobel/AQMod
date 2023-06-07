using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Common.Building {
    public struct ScanInfo {
        public readonly Rectangle Area;
        public ScanResults results;

        public int X => Area.X;
        public int Y => Area.Y;
        public int Width => Area.Width;
        public int Height => Area.Height;

        public Tile this[int x, int y] {
            get => Framing.GetTileSafely(x + X, y + Y);
        } 

        public ScanInfo(Rectangle area) {
            Area = area;
            results = new(area);
        }
    }
}