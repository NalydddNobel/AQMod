namespace Aequus.Common.Drawing;

public interface IGridDrawSystem : IDrawSystem {
    bool Accept(Point p) {
        return true;
    }
    bool InBounds(Point p, Rectangle Bounds) {
        return Bounds.Contains(p);
    }
}
