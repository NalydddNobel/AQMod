using Aequus.Common.Entities.TileActors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Aequus.Common.Drawing;

[Autoload(Side = ModSide.Client)]
public class TileDrawSystem : ModSystem {
    readonly Dictionary<ITileDrawSystem, List<Point>> _systems = [];
    readonly Dictionary<GridActor, List<GridActor>> _gridActors = [];
    Rectangle _bounds;

    public override void ClearWorld() {
        _bounds = Rectangle.Empty;

        foreach (var p in _systems.Values) {
            p.Clear();
        }
        foreach (var a in _gridActors.Values) {
            a.Clear();
        }
    }

    public override void PostDrawTiles() {
        int offScreenRange = Main.offScreenRange * 2 / 16;
        Rectangle bounds = new Rectangle((int)(Main.screenPosition.X / 16 - offScreenRange), (int)(Main.screenPosition.Y / 16 - offScreenRange), Main.screenWidth / 16 + offScreenRange * 2, Main.screenHeight / 16 + offScreenRange * 2);

        if (!_bounds.Equals(bounds)) {
            ClearCaches(bounds);
            UpdateTileCaches(bounds);
        }

        _bounds = bounds;
    }

    void ClearCaches(Rectangle bounds) {
        foreach (var pairs in _systems.Where(p => p.Value.Count > 0)) {
            pairs.Value.RemoveAll((p) => !Main.tile[p].HasTile || Main.tile[p].TileType != pairs.Key.Type || !pairs.Key.InBounds(p, bounds));

            if (pairs.Value.Count == 0) {
                pairs.Key.Deactivate();
            }
        }

        foreach (var pairs in _gridActors.Where(a => a.Value.Count > 0)) {
            pairs.Value.RemoveAll(a => {
                IGridDrawSystem draw = (a as IGridDrawSystem)!;

                return !draw.InBounds(a.Location, bounds) || !draw.Accept(a.Location);
            });

            if (pairs.Value.Count == 0) {
                (pairs.Key as IGridDrawSystem)!.Deactivate();
            }
        }
    }

    void UpdateTileCaches(Rectangle bounds) {
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

        GridActorSystem actors = Instance<GridActorSystem>();
        int sectionX = GridActorSystem.SectionX;
        int sectionY = GridActorSystem.SectionY;
        for (int i = bounds.X / sectionX * sectionX; i < right; i += sectionX) {
            for (int j = bounds.Y / sectionY * sectionY; j < bottom; j += sectionY) {
                if (!actors.TryGetActorsInSection(i, j, out List<GridActor> list)) {
                    continue;
                }

                foreach (GridActor info in list) {
                    if (!bounds.Contains(info.Location)) {
                        continue;
                    }

                    AddGridActor(info);
                }
            }
        }
    }

    internal void Add(ITileDrawSystem system, Point point) {
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

    internal void Remove(ITileDrawSystem system, Point point) {
        if (!_systems.TryGetValue(system, out List<Point>? points)) {
            return;
        }

        points.Remove(point);

        if (points.Count == 0) {
            system.Deactivate();
        }
    }

    internal void AddGridActor(GridActor instance) {
        GridActor key = GridActorSystem.Registered[instance.Type];
        List<GridActor> drawList = (CollectionsMarshal.GetValueRefOrAddDefault(_gridActors, key, out _) ??= new());

        if (drawList.Contains(instance)) {
            return;
        }

        if (drawList.Count == 0) {
            (key as IGridDrawSystem)!.Activate();
        }

        drawList.Add(instance);
    }

    internal void RemoveGridActor(GridActor instance) {
        var key = GridActorSystem.Registered[instance.Type];
        _gridActors[key].Remove(instance);
    }

    internal IEnumerable<Point> GetDrawPoints(ITileDrawSystem system) {
        return !_systems.TryGetValue(system, out List<Point>? points) ? Array.Empty<Point>() : points!;
    }

    internal IEnumerable<T> GetDrawableActors<T>(GridActor system) where T : GridActor, IGridDrawSystem {
        return !_gridActors.TryGetValue(system, out List<GridActor>? points) ? Array.Empty<T>() : points!.Cast<T>();
    }
}

[Autoload(Side = ModSide.Client)]
public class TileDrawSystemGlobalTile : GlobalTile {
    public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak) {
        if (Main.netMode != NetmodeID.Server && TileLoader.GetTile(type) is ITileDrawSystem system) {
            Point nextPoint = new Point(i, j);
            if (system.Accept(nextPoint)) {
                Instance<TileDrawSystem>().Add(system, nextPoint);
            }
        }
        return true;
    }

    public override void PlaceInWorld(int i, int j, int type, Item item) {
        if (Main.netMode != NetmodeID.Server && TileLoader.GetTile(type) is ITileDrawSystem system) {
            Tile tile = Main.tile[i, j];
            TileHelper.GetTileCorner(i, j, out int left, out int top);

            Point nextPoint = new Point(left, top);
            if (system.Accept(nextPoint)) {
                Instance<TileDrawSystem>().Add(system, nextPoint);
            }
        }
    }

    public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
        if (Main.netMode != NetmodeID.Server && TileLoader.GetTile(type) is ITileDrawSystem system) {
            Point nextPoint = new Point(i, j);
            if (system.Accept(nextPoint)) {
                Instance<TileDrawSystem>().Remove(system, nextPoint);
            }
        }
    }
}