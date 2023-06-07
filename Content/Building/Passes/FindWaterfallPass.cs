using Aequus.Common.Building;
using Aequus.Common.Building.Passes;

namespace Aequus.Content.Building.Passes {
    public class FindWaterfallPass : TileScanPass {
        private void Waterfall(int i, int j, int xDir, ref ScanResults scanResults, ref ScanInfo info) {
            for (int k = 0; k < 80; k++) {
                scanResults.ShapeMap.SafeSet(i, j, true);
                if (info[i, j].LiquidAmount > 0) {
                    return;
                }

                if (info[i, j + 1].IsFullySolid()) {
                    if (xDir == 0) {
                        return;
                    }
                    if (info[i + xDir, j].IsFullySolid() && !info[i, j].IsHalfBlock) {
                        // Loop
                        if (info[i - xDir, j].IsFullySolid() && !info[i, j].IsHalfBlock) {
                            return;
                        }

                        xDir = -xDir;
                    }
                    i += xDir;
                    continue;
                }
                j++;
            }
        }

        private bool IsSolidHalfBrick(int i, int j, ref ScanInfo info) {
            return info[i, j].IsFullySolid() && info[i, j].IsHalfBlock;
        }

        private void CheckWaterTile(int i, int j, ref ScanResults scanResults, ref ScanInfo info) {
            if (IsSolidHalfBrick(i - 1, j, ref info)) {
                Waterfall(i - 1, j, -1, ref scanResults, ref info);
            }
            if (IsSolidHalfBrick(i + 1, j, ref info)) {
                Waterfall(i + 1, j, 1, ref scanResults, ref info);
            }
            if (IsSolidHalfBrick(i, j + 1, ref info)) {
                Waterfall(i, j + 1, 0, ref scanResults, ref info);
            }
        }

        public override ScanResults Scan(ref ScanInfo info) {
            ScanResults scanResults = new(info.Area);
            for (int i = 0; i < info.Width; i++) {
                for (int j = 0; j < info.Height; j++) {
                    if (info[i, j].LiquidAmount <= 0) {
                        continue;
                    }

                    CheckWaterTile(i, j, ref scanResults, ref info);
                }
            }
            return scanResults;
        }
    }
}