using Microsoft.Xna.Framework;

namespace Aequus.Common.Building {
    public struct ScanResults {
        public ScanMap<bool> InterestMap;
        public ScanMap<bool> ShapeMap;
        public ScanMap<bool> ErrorMap;
        public string Report;

        public ScanResults(int width, int height) {
            InterestMap = new(width, height);
            ShapeMap = new(width, height);
            ErrorMap = new(width, height);
        }
        public ScanResults(Rectangle area) : this(area.Width, area.Height) {
        }
    }
}