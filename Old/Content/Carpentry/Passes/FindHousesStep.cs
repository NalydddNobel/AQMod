using Aequus.Common.Carpentry.Results;
using Aequus.Old.Common.Carpentry;
using Aequus.Old.Common.Carpentry.Results;
using Aequus.Old.Content.Building.Carpentry.Results;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace Aequus.Old.Content.Carpentry.Passes;

public class FindHousesStep : StepRequirement<ScanInfo, FindHousesStep.Parameters> {
    public enum SearchResultType {
        CountHouses,
        CountMostWalls,
        CountTotalWalls
    }

    public record struct Parameters(int RequiredAmount, int RequiredToCountAsHouse, Rectangle InputRectangle, ScanMap<bool> outputValidTiles, ScanMap<bool> outputInvalidTiles, SearchResultType ResultType) : IStepRequirementParameters {
        public List<List<Point>> FoundHouses = new();
    }

    private static List<Point> FindHousingWalls(int x, int y, Rectangle bounds) {
        var addPoints = new List<Point>();
        var checkedPoints = new List<Point>() { new Point(x, y) };
        var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
        for (int k = 0; k < 1000; k++) {
            checkedPoints.AddRange(addPoints);
            addPoints.Clear();
            bool addedAny = false;
            if (checkedPoints.Count > 1000) {
                return checkedPoints;
            }
            for (int l = 0; l < checkedPoints.Count; l++) {
                for (int m = 0; m < offsets.Length; m++) {
                    var newPoint = new Point(checkedPoints[l].X + offsets[m].X, checkedPoints[l].Y + offsets[m].Y);
                    if (!bounds.Contains(newPoint)) {
                        continue;
                    }

                    var tile = Framing.GetTileSafely(newPoint);
                    if (!checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) && tile.WallType != WallID.None && Main.wallHouse[tile.WallType]) {
                        var slopeType = SlopeType.Solid;
                        if (offsets[m].X != 0) {
                            slopeType = tile.Slope;
                            if (TileID.Sets.Platforms[tile.TileType]) {
                                if (tile.TileFrameX == 144) {
                                    slopeType = SlopeType.SlopeDownLeft;
                                }
                                if (tile.TileFrameX == 180) {
                                    slopeType = SlopeType.SlopeDownRight;
                                }
                            }
                        }

                        if (!tile.HasTile || (!tile.IsSolid() || tile.SolidTopType()) && (!tile.IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor) || slopeType != SlopeType.Solid)) {
                            for (int n = 0; n < offsets.Length; n++) {
                                var checkWallPoint = newPoint + offsets[n];
                                var checkTile = Framing.GetTileSafely(checkWallPoint);
                                if (!checkTile.IsFullySolid() && (checkTile.WallType == WallID.None || !Main.wallHouse[checkTile.WallType])) {
                                    continue;
                                    //return new List<Point>() { new Point(x, y) };
                                }
                            }
                            addPoints.Add(newPoint);
                            addedAny = true;
                        }
                    }
                }
            }
            if (!addedAny) {
                return checkedPoints;
            }
        }
        return checkedPoints;
    }

    public override IScanResults GetStepResults(in ScanInfo info, in Parameters parameters = default(Parameters)) {
        int totalHouses = 0;
        int highestHouseWalls = 0;
        int totalHouseWalls = 0;
        for (int i = 0; i < parameters.InputRectangle.Width; i++) {
            for (int j = 0; j < parameters.InputRectangle.Height; j++) {
                int x = i + parameters.InputRectangle.X;
                int y = j + parameters.InputRectangle.Y;

                var tile = Framing.GetTileSafely(x, y);
                if (tile.IsFullySolid() || tile.WallType == WallID.None || !Main.wallHouse[tile.WallType] || tile.IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor) || parameters.outputValidTiles[i, j] || parameters.outputInvalidTiles[i, j]) {
                    continue;
                }

                var pendingList = FindHousingWalls(x, y, parameters.InputRectangle);
                totalHouseWalls += pendingList.Count;
                highestHouseWalls = Math.Max(pendingList.Count, highestHouseWalls);
                var outputList = parameters.outputValidTiles;
                if (pendingList.Count >= parameters.RequiredToCountAsHouse) {
                    totalHouses++;
                    parameters.FoundHouses.Add(pendingList);
                }
                else {
                    outputList = parameters.outputInvalidTiles;
                }
                foreach (var point in pendingList) {
                    outputList.SafeSet(point.X - parameters.InputRectangle.X, point.Y - parameters.InputRectangle.Y, true);
                }
            }
        }

        return parameters.ResultType switch {
            SearchResultType.CountTotalWalls => new StepResultRatioPercent(totalHouseWalls, parameters.RequiredAmount),
            SearchResultType.CountMostWalls => new StepResultRatioPercent(highestHouseWalls, parameters.RequiredAmount),
            _ => new StepResultRatio(totalHouses, parameters.RequiredAmount)
        };
    }
}