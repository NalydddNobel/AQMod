using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Building.Passes;
public class ActuatorDoorStep : StepRequirement<ScanInfo, ActuatorDoorStep.Parameters> {
    public record struct Parameters(int RequiredAmount, List<List<Point>> Houses, ScanMap<bool> outputValidTiles, ScanMap<bool> outputInvalidTiles) : IStepRequirementParameters {
        public List<List<Point>> FoundHouses = new();
        public Point actuatorDoor = new(-1, -1);
        public List<Point> TriggerPoints = new();
        public bool isProbablyAValidDoor = false;
    }

    public bool CheckActuatorDoor(int x, int y, out List<Point> actuatorDoorPoints) {
        var list = new List<Point>() { new(x, y), };
        var alreadyChecked = new List<Point>();
        actuatorDoorPoints = new();
        while (list.Count > 0) {
            var p = list[0];
            list.RemoveAt(0);
            if (!WorldGen.InWorld(p.X, p.Y)) {
                alreadyChecked.Add(p);
                continue;
            }

            var t = Main.tile[p];
            if (t.IsFullySolid() && t.HasActuator && !alreadyChecked.Contains(p)) {
                list.Add(new Point(p.X, p.Y - 1));
                list.Add(new Point(p.X, p.Y + 1));
                list.Add(new Point(p.X - 1, p.Y));
                list.Add(new Point(p.X + 1, p.Y));
                actuatorDoorPoints.Add(p);
            }
            alreadyChecked.Add(p);
        }
        return true;
    }

    public void FindActuatorDoorTriggers(int x, int y, out List<Point> triggerPoints) {
        triggerPoints = new();
        for (int wireID = 0; wireID < 4; wireID++) {
            var list = new List<Point>() { new(x, y), };
            var alreadyChecked = new List<Point>();
            while (list.Count > 0) {
                var p = list[0];
                if (!WorldGen.InWorld(list[0].X, list[0].Y)) {
                    alreadyChecked.Add(list[0]);
                    list.RemoveAt(0);
                    continue;
                }
                var t = Main.tile[p];
                if (TileDataPacking.GetBit(t.Get<TileWallWireStateData>().WireData, wireID) && !alreadyChecked.Contains(p)) {
                    list.Add(new Point(p.X + 1, p.Y));
                    list.Add(new Point(p.X - 1, p.Y));
                    list.Add(new Point(p.X, p.Y + 1));
                    list.Add(new Point(p.X, p.Y - 1));
                    if (t.HasTile && TileID.Sets.IsATrigger[t.TileType]) {
                        triggerPoints.Add(p);
                    }
                }
                alreadyChecked.Add(list[0]);
                list.RemoveAt(0);
            }
        }
    }

    public override IStepResults GetStepResults(in ScanInfo info, in Parameters parameters = default(Parameters)) {
        parameters.TriggerPoints?.Clear();
        var offsets = new Point[4] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
        int totalActuatorDoors = 0;
        var infoOffset = new Point(-info.X, -info.Y);
        foreach (var house in parameters.Houses) {
            foreach (var anchorPoint in house) {
                foreach (var off in offsets) {
                    var checkPoint = anchorPoint + off;
                    var infoPoint = checkPoint + infoOffset;
                    if (!WorldGen.InWorld(checkPoint.X, checkPoint.Y) || !parameters.outputValidTiles.SafeGet(infoPoint.X, infoPoint.Y, out bool value) || value || !parameters.outputInvalidTiles.SafeGet(infoPoint.X, infoPoint.Y, out value) || value) {
                        continue;
                    }

                    if (Main.tile[checkPoint].HasActuator && Main.tile[checkPoint].IsFullySolid()) {
                        if (CheckActuatorDoor(checkPoint.X, checkPoint.Y, out var actuatorDoorPoints)) {
                            FindActuatorDoorTriggers(checkPoint.X, checkPoint.Y, out var triggerPoints);
                            if (triggerPoints.Count > 0) {
                                foreach (var t in triggerPoints) {
                                    parameters.outputValidTiles.SafeSet(t.X + infoOffset.X, t.Y + infoOffset.Y, true);
                                }
                                foreach (var t in actuatorDoorPoints) {
                                    parameters.outputValidTiles.SafeSet(t.X + infoOffset.X, t.Y + infoOffset.Y, true);
                                }
                                totalActuatorDoors++;
                                continue;
                            }

                            foreach (var t in actuatorDoorPoints) {
                                parameters.outputInvalidTiles.SafeSet(t.X + infoOffset.X, t.Y + infoOffset.Y, true);
                            }
                        }
                    }
                }
            }
        }

        return new StepResultRatio(totalActuatorDoors, parameters.RequiredAmount);
    }
}
