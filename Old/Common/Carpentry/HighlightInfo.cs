using Microsoft.Xna.Framework;

namespace Aequus.Old.Common.Carpentry {
    public struct HighlightInfo {
        public ScanMap<bool> InterestMap;
        public ScanMap<bool> ShapeMap;
        public ScanMap<bool> ErrorMap;
        public string Report;
        public float PercentCompletion;

        public HighlightInfo(int width, int height) {
            InterestMap = new(width, height);
            ShapeMap = new(width, height);
            ErrorMap = new(width, height);
            Report = null;
            PercentCompletion = 0f;
        }
        public HighlightInfo(Rectangle area) : this(area.Width, area.Height) {
        }
    }
}