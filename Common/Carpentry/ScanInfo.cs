using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Common.Carpentry {
    public readonly struct ScanInfo : IScanInfo {
        public readonly Rectangle Area;

        public int X => Area.X;
        public int Y => Area.Y;
        public int Width => Area.Width;
        public int Height => Area.Height;

        Rectangle IScanInfo.Area => Area;

        public Tile this[int x, int y] {
            get => Framing.GetTileSafely(x + X, y + Y);
        } 

        public ScanInfo(Rectangle area) {
            Area = area;
        }
    }
}