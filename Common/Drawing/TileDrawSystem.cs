using System;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.Drawing;

public class TileDrawSystem : ModSystem {
    private static readonly Dictionary<ITileDrawSystem, List<Point>> _systems = [];
    private static Rectangle _bounds;

    public override void ClearWorld() {
        _bounds = Rectangle.Empty;
    }

    public override void PostDrawTiles() {
        int offScreenRange = Main.offScreenRange * 2 / 16;
        Rectangle bounds = new Rectangle((int)(Main.screenPosition.X / 16 - offScreenRange), (int)(Main.screenPosition.Y / 16 - offScreenRange), Main.screenWidth / 16 + offScreenRange * 2, Main.screenHeight / 16 + offScreenRange * 2);

        if (!_bounds.Equals(bounds)) {
            UpdateTileCaches(bounds);
        }

        _bounds = bounds;
    }

    static void UpdateTileCaches(Rectangle bounds) {
        foreach (var pairs in _systems.Where(p => p.Value.Count > 0)) {
            pairs.Value.RemoveAll((p) => !Main.tile[p].HasTile || Main.tile[p].TileType != pairs.Key.Type || !pairs.Key.InBounds(p, bounds));

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

        if (points.Count == 0) {
            system.Activate();
        }

        if (!points.Contains(point)) {
            points.Add(point);
        }
    }

    internal static void Remove(ITileDrawSystem system, Point point) {
        if (!_systems.TryGetValue(system, out List<Point>? points)) {
            return;
        }

        points.Remove(point);

        if (points.Count == 0) {
            system.Deactivate();
        }
    }

    internal static IEnumerable<Point> GetDrawPoints(ITileDrawSystem system) {
        return !_systems.TryGetValue(system, out List<Point>? points) ? Array.Empty<Point>() : points!;
    }
}

public class TileDrawSystemGlobalTile : GlobalTile {
    public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak) {
        if (Main.netMode != NetmodeID.Server && TileLoader.GetTile(type) is ITileDrawSystem system) {
            Point nextPoint = new Point(i, j);
            if (system.Accept(nextPoint)) {
                TileDrawSystem.Add(system, nextPoint);
            }
        }
        return true;
    }

    public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
        if (Main.netMode != NetmodeID.Server && TileLoader.GetTile(type) is ITileDrawSystem system) {
            Point nextPoint = new Point(i, j);
            if (system.Accept(nextPoint)) {
                TileDrawSystem.Remove(system, nextPoint);
            }
        }
    }
}