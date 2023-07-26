using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Building.old.Steps {
    public class ActuatorDoorStep : Step {
        public class Interest : StepInterest {
            public Point actuatorDoor;
            public List<Point> TriggerPoints;
            public bool isProbablyAValidDoor;
            public Dictionary<Point, List<Point>> givenHouses;

            public override void CompileInterestingPoints(StepInfo info) {
                actuatorDoor = new Point(-1, -1);
                TriggerPoints?.Clear();
                TriggerPoints = new List<Point>();
                isProbablyAValidDoor = false;
                var offsets = new Point[4] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
                foreach (var house in givenHouses.Values) {
                    foreach (var anchorPoint in house) {
                        foreach (var off in offsets) {
                            var checkPoint = anchorPoint + off;
                            if (info.Map.InSceneRenderedMap(checkPoint)) {
                                if (info[checkPoint].HasActuator && info[checkPoint].IsFullySolid) {
                                    actuatorDoor.X = checkPoint.X;
                                    actuatorDoor.Y = checkPoint.Y;
                                    if (CheckActuatorDoor(info)) {
                                        FindActuatorDoorTriggers(info);
                                        if (TriggerPoints.Count > 0) {
                                            isProbablyAValidDoor = true;
                                            return;
                                        }
                                        TriggerPoints.Clear();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public bool CheckActuatorDoor(StepInfo info) {
                var list = new List<Point>() { actuatorDoor, };
                var alreadyChecked = new List<Point>();
                int amt = 0;
                while (list.Count > 0) {
                    var p = list[0];
                    if (!info.Map.InSceneRenderedMap(list[0])) {
                        alreadyChecked.Add(list[0]);
                        list.RemoveAt(0);
                        continue;
                    }
                    var t = info[p];
                    if (t.IsFullySolid && t.HasActuator && !alreadyChecked.Contains(p)) {
                        amt++;
                        list.Add(new Point(p.X, p.Y - 1));
                        list.Add(new Point(p.X, p.Y + 1));
                    }
                    alreadyChecked.Add(list[0]);
                    list.RemoveAt(0);
                }
                return true;
            }

            public void FindActuatorDoorTriggers(StepInfo info) {
                for (int wireID = 0; wireID < 4; wireID++) {
                    var list = new List<Point>() { actuatorDoor, };
                    var alreadyChecked = new List<Point>();
                    while (list.Count > 0) {
                        var p = list[0];
                        if (!info.Map.InSceneRenderedMap(list[0])) {
                            alreadyChecked.Add(list[0]);
                            list.RemoveAt(0);
                            continue;
                        }
                        var t = info[p];
                        if (TileDataPacking.GetBit(t.WireData, wireID) && !alreadyChecked.Contains(p)) {
                            list.Add(new Point(p.X + 1, p.Y));
                            list.Add(new Point(p.X - 1, p.Y));
                            list.Add(new Point(p.X, p.Y + 1));
                            list.Add(new Point(p.X, p.Y - 1));
                            if (t.HasTile && TileID.Sets.IsATrigger[t.TileType]) {
                                TriggerPoints.Add(p);
                            }
                        }
                        alreadyChecked.Add(list[0]);
                        list.RemoveAt(0);
                    }
                }
            }
        }

        protected override void Init(StepInfo info) {
            info.AddInterest(new Interest());
        }

        protected override StepResult ProvideResult(StepInfo info) {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            return new StepResult("NoActuatorDoor") {
                success = interest.isProbablyAValidDoor,
                interest = new List<Point>(interest.TriggerPoints) { interest.actuatorDoor, }
            };
        }
    }
}