namespace Aequus.Common.Drawing;
public interface ITileDrawSystem : IDrawSystem {
    int Type { get; }

    bool Accept(Point p) {
        return true;
    }
    bool InBounds(Point p, Rectangle Bounds) {
        return Bounds.Contains(p);
    }
}
