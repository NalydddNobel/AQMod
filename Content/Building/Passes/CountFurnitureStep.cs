using Aequus.Common.Building;
using Aequus.Common.Building.Results;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ObjectData;

namespace Aequus.Content.Building.Passes;

public class CountFurnitureStep : StepRequirement<ScanInfo, CountFurnitureStep.Parameters> {
    public record struct Parameters(int RequiredFurniture, ScanMap<bool> outputValidTiles, ScanMap<bool> outputInvalidTiles, bool RequireUniqueStyles = true) : IStepRequirementParameters {
        public List<Rectangle> CheckRectangles = new();
        public List<Point> CheckPoints = new();

        public Parameters AddRectangle(Rectangle rectangle) {
            CheckRectangles.Add(rectangle);
            return this;
        }

        public Parameters AddPoint(Point point) {
            CheckPoints.Add(point);
            return this;
        }

        public Parameters AddPoints(List<Point> points) {
            CheckPoints.AddRange(points);
            return this;
        }

        public Parameters AddPoints(IEnumerable<List<Point>> points) {
            foreach (var pointList in points) {
                AddPoints(pointList);
            }
            return this;
        }
    }

    private bool CheckPoint(int x, int y, Dictionary<int, List<int>> tileStyleData, List<Point> repeatPoints, in Parameters parameters) {
        var tile = Framing.GetTileSafely(x, y);
        if (!tile.HasTile || tile.IsSolid()) {
            return false;
        }

        if (parameters.RequireUniqueStyles) {
            int style = Helper.GetTileStyle(tile.TileType, tile.TileFrameX, tile.TileFrameY);
            if (tileStyleData.TryGetValue(tile.TileType, out List<int> compareStyle)) {
                if (compareStyle.Contains(style)) {
                    repeatPoints.Add(new Point(x, y));
                    return false;
                }
                compareStyle.Add(style);
            }
            else {
                tileStyleData.Add(tile.TileType, new List<int>() { style });
            }
        }
        return true;
    }

    public override IStepResults GetStepResults(in ScanInfo info, in Parameters parameters = default(Parameters)) {
        Dictionary<int, List<int>> tileStyleData = new();
        List<Point> furniturePoints = new();
        List<Point> repeatPoints = new();

        foreach (var point in parameters.CheckPoints) {
            if (!repeatPoints.Contains(point) && CheckPoint(point.X, point.Y, tileStyleData, repeatPoints, in parameters)) {
                var data = TileObjectData.GetTileData(Framing.GetTileSafely(point));
                if (data == null) {
                    continue;
                }
                furniturePoints.Add(point);
            }
        }
        foreach (var rectangle in parameters.CheckRectangles) {
            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++) {
                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++) {
                    if (!repeatPoints.Contains(new(i, j)) && CheckPoint(i, j, tileStyleData, repeatPoints, in parameters)) {
                        var data = TileObjectData.GetTileData(Framing.GetTileSafely(i, j));
                        if (data == null) {
                            continue;
                        }
                        furniturePoints.Add(new(i, j));
                    }
                }
            }
        }

        var output = parameters.outputValidTiles;
        foreach (var point in furniturePoints) {
            int i = point.X - info.X;
            int j = point.Y - info.Y;
            var data = TileObjectData.GetTileData(Framing.GetTileSafely(point));
            if (data == null) {
                continue; // ???
            }

            for (int k = i; k < i + data.Width; k++) {
                for (int l = j; l < j + data.Height; l++) {
                    output.SafeSet(k, l, true);
                }
            }
        }
        return new StepResultRatio(furniturePoints.Count, parameters.RequiredFurniture);
    }
}