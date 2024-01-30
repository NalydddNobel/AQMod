using System;
using System.Collections.Generic;

namespace Aequus.Content.Tiles.Conductive;
public class ConductiveSystem : ModSystem {
    public class ActivationEffect {
        public Single electricAnimation;
        public Single intensity;
        public Int32 timeActive;

        public void Update() {
            electricAnimation = MathF.Sin(timeActive / (Single)WireMax / 4f * MathHelper.TwoPi);
            timeActive++;
        }

        public static Single GetDistance(Int32 i, Int32 j) {
            Int32 differenceX = Math.Abs(PoweredLocation.X - i);
            Int32 differenceY = Math.Abs(PoweredLocation.Y - j);
            return Math.Max(differenceX, differenceY);
        }

        public static void Activate(Int32 i, Int32 j, Single distance) {
            var point = new Point(i, j);
            Single intensity = 1f - Math.Abs(distance / WireMax);
            if (ActivationPoints.TryGetValue(point, out var effect)) {
                effect.timeActive = Math.Max(effect.timeActive, (Int32)distance);
                effect.intensity = Math.Max(effect.intensity, intensity);
            }
            else {
                ActivationPoints[point] = new() { timeActive = (Int32)distance, intensity = intensity };
            }
        }
    }

    public static Point PoweredLocation;
    public static Int32 WireMax = 8;
    public static Int32 ActivationDelay = 60;

    public static readonly Dictionary<Point, ActivationEffect> ActivationPoints = new();
    public static Vector2[] ElectricOffsets = new Vector2[7];
    private static readonly List<Point> RemoveActivationPoints = new();

    public override void ClearWorld() {
        ActivationPoints.Clear();
        RemoveActivationPoints.Clear();
    }

    public override void PreUpdateEntities() {
        if (ActivationPoints.Count < 0) {
            return;
        }

        PoweredLocation = Point.Zero;
        for (Int32 i = 0; i < ElectricOffsets.Length; i++) {
            ElectricOffsets[i] = Main.rand.NextVector2Square(-i, i);
        }
        foreach (var p in ActivationPoints) {
            p.Value.Update();
            if (p.Value.timeActive > WireMax * 4) {
                RemoveActivationPoints.Add(p.Key);
            }
        }
        foreach (var p in RemoveActivationPoints) {
            ActivationPoints.Remove(p);
        }
        RemoveActivationPoints.Clear();
    }
}