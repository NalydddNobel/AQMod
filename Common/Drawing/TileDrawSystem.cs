using System;
using System.Collections.Generic;

namespace Aequus.Common.Drawing;

public class TileDrawSystem : ModSystem {
    private static readonly Dictionary<ITileDrawSystem, List<Point>> _systems = [];
    private static Rectangle _bounds;

    public override void ClearWorld() {
        _bounds = Rectangle.Empty;
    }

    public override void PostDrawTiles() {
        Rectangle bounds = new Rectangle((int)((Main.screenPosition.X - Main.offScreenRange) / 16), (int)((Main.screenPosition.Y - Main.offScreenRange) / 16), (Main.screenWidth + Main.offScreenRange) / 16, (Main.screenHeight + Main.offScreenRange) / 16);

        if (!_bounds.Equals(bounds)) {
            UpdateTileCaches(bounds);
        }

        _bounds = bounds;
    }

    static void UpdateTileCaches(Rectangle bounds) {
        foreach (var pairs in _systems) {
            pairs.Value.RemoveAll((p) => !pairs.Key.InBounds(p, bounds));

            if (pairs.Value.Count == 0) {
                pairs.Key.Deactivate();
            }
        }

        int right = bounds.X + bounds.Width;
        int bottom = bounds.Y + bounds.Height;
        for (int i = bounds.X; i < right; i++) {
            for (int j = bounds.Y; j < bottom; j++) {
                Point nextPoint = new Point(i, j);
                Tile tile = Framing.GetTileSafely(nextPoint);
                ModTile modTile = TileLoader.GetTile(tile.TileType);
                if (modTile is ITileDrawSystem nextSystem && nextSystem.Accept(nextPoint)) {
                    Add(nextSystem, nextPoint);
                }
            }
        }
    }

    internal static void Add(ITileDrawSystem system, Point point) {
        if (!_systems.TryGetValue(system, out List<Point>? points)) {
            points = _systems[system] = new(64);
        }
        //List<Point> points = _systems[system] ??= new(64);

        if (points.Count == 0) {
            system.Activate();
        }

        if (!points.Contains(point)) {
            points.Add(point);
        }
    }

    internal static IEnumerable<Point> GetDrawPoints(ITileDrawSystem system) {
        return !_systems.TryGetValue(system, out List<Point>? points) ? Array.Empty<Point>() : points!;
    }
}

public static class TileDrawSystemExtensions {
    public static IEnumerable<Point> GetDrawPoints<T>(this T system) where T : ITileDrawSystem {
        return TileDrawSystem.GetDrawPoints(system);
    }
}