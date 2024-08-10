using System.Collections.Generic;

namespace Aequus.Common.Drawing;

public static class TileDrawSystemExtensions {
    public static IEnumerable<Point> GetDrawPoints<T>(this T system) where T : ITileDrawSystem {
        return TileDrawSystem.GetDrawPoints(system);
    }
}