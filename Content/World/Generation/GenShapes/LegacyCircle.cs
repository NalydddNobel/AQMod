using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace Aequus.Content.World.Generation.GenShapes {
    [Obsolete("Replace with Terraria standard gen shapes.")]
    public struct LegacyCircle {
        public static LegacyCircle Invalid => new LegacyCircle(-1, -1, -1);

        public int X;
        public int Y;
        public int Radius;

        public bool IsInvalid => X == -1;

        public LegacyCircle(int x, int y, int radius) {
            X = x;
            Y = y;
            Radius = radius;
        }

        public bool Inside(int x, int y) {
            int x2 = x - X;
            int y2 = y - Y;
            return Math.Sqrt(x2 * x2 + y2 * y2) <= Radius;
        }

        public double Distance(int x, int y) {
            int x2 = x - X;
            int y2 = y - Y;
            return Math.Sqrt(x2 * x2 + y2 * y2);
        }

        public LegacyCircle GetRandomCircleInsideCircle(int minDistanceFromEdge, int minScale, int maxScale, UnifiedRandom rand, Func<LegacyCircle, bool> isValid) {
            var testPoints = new List<Point>();
            for (int i = 0; i < Radius * 2; i++) {
                for (int j = 0; j < Radius * 2; j++) {
                    int x = X + i - Radius;
                    int y = Y + j - Radius;
                    if (Distance(x, y) < Radius - minDistanceFromEdge) {
                        if (!Main.tile[x, y].HasTile || !Main.tile[x, y].SolidType()) {
                            continue;
                        }
                        testPoints.Add(new Point(x, y));
                    }
                }
            }
            for (int i = 0; i < testPoints.Count; i++) {
                int chosenPoint = rand.Next(testPoints.Count);
                int size = rand.Next(minScale, maxScale);
                for (int j = size; j >= minScale; j--) {
                    var c = FixedCircle(testPoints[chosenPoint].X, testPoints[chosenPoint].Y, j);
                    if (isValid?.Invoke(c) != false) {
                        return c;
                    }
                }
                testPoints.RemoveAt(i);
            }
            return Invalid;
        }

        public LegacyCircle GetRandomCircleInsideCircleNoAirCheck(int minDistanceFromEdge, int minScale, int maxScale, UnifiedRandom rand) {
            List<Point> testPoints = new List<Point>();
            for (int i = 0; i < Radius * 2; i++) {
                for (int j = 0; j < Radius * 2; j++) {
                    int x = X + i - Radius;
                    int y = Y + j - Radius;
                    if (Distance(x, y) < Radius - minDistanceFromEdge) {
                        if (!Main.tile[x, y].HasTile || !Main.tile[x, y].SolidType()) {
                            continue;
                        }
                        testPoints.Add(new Point(x, y));
                    }
                }
            }
            int chosenPoint = rand.Next(testPoints.Count);
            int size = rand.Next(minScale, maxScale);
            return FixedCircle(testPoints[chosenPoint].X, testPoints[chosenPoint].Y, size);
        }

        public static LegacyCircle FixedCircle(int x, int y, int radius) {
            if (x - radius < 10) {
                x = radius + 10;
            }
            else if (x + radius > Main.maxTilesX - 10) {
                x = Main.maxTilesX - 10 - radius;
            }
            if (y - radius < 10) {
                y = radius + 10;
            }
            else if (y + radius > Main.maxTilesY - 10) {
                y = Main.maxTilesY - 10 - radius;
            }
            return new LegacyCircle(x, y, radius);
        }
    }
}