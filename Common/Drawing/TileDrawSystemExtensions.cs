using Aequus.Common.Entities.TileActors;
using System.Collections.Generic;

namespace Aequus.Common.Drawing;

public static class TileDrawSystemExtensions {
    public static IEnumerable<Point> GetDrawPoints<T>(this T system) where T : ITileDrawSystem {
        return Instance<TileDrawSystem>().GetDrawPoints(system);
    }

    public static IEnumerable<T> GetDrawable<T>(this T gridActor) where T : GridActor, IGridDrawSystem {
        return Instance<TileDrawSystem>().GetDrawableActors<T>(gridActor);
    }
}